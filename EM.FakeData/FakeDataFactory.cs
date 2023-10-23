using EM.Contracts;
using Bogus;

namespace EM.FakeData;

public static class FakeDataFactory
{
    public static List<Offer> Offers = new();
    public static List<OfferDescription> OfferDescriptions = new();
    public static List<Category> Categories = new();
    public static List<Place> Places = new();
    public static List<GeoTag> GeoTags = new();
    public static List<Organization> Organizations = new();
    public static List<User> Users = new();
    public static List<UserWithRole> UsersWithRole = new();

    public static void Init()
    {
        var categoryFaker = new Faker<Category>("ru")
            .RuleFor(c => c.Id, f => f.Random.Guid())
            .RuleFor(c => c.Created, f => f.Date.Past())
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0]);

        var categories = categoryFaker.Generate(10);
        Categories.AddRange(categories);

        var geoTagFaker = new Faker<GeoTag>("ru")
            .RuleFor(gt => gt.Id, f => f.Random.Guid())
            .RuleFor(gt => gt.Latitude, f => f.Address.Latitude(50, 60))
            .RuleFor(gt => gt.Longitude, f => f.Address.Longitude(32, 150));

        var placeFaker = new Faker<Place>("ru")
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(3))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence(6))
            .RuleFor(p => p.Region, f => f.Address.State())
            .RuleFor(p => p.City, f => f.Address.City())
            .RuleFor(p => p.GeoTag, (f, p) =>
            {
                var geoTag = geoTagFaker.Generate(1)[0];
                GeoTags.Add(geoTag);
                return geoTag;
            })
            .RuleFor(p => p.Address, f => f.Address.StreetAddress());

        var offerDescriptionFaker = new Faker<OfferDescription>("ru")
            .RuleFor(od => od.Id, f => f.Random.Guid())
            .RuleFor(od => od.Title, f => f.Commerce.ProductName())
            .RuleFor(od => od.Description, f => f.Lorem.Sentences(5))
            .RuleFor(od => od.CategoryId, f => Categories[f.Random.Int(1, Categories.Count - 1)].Id)
            .RuleFor(od => od.IsSale, f => f.Random.Bool())
            .RuleFor(od => od.Image, f => f.Image.PicsumUrl())
            .RuleFor(od => od.MetricUnit, f => f.PickRandom<MetricUnit>())
            .RuleFor(od => od.Created, f => f.Date.Past());

        var offerFaker = new Faker<Offer>("ru")
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.IsInfinitiveResource, f => f.Random.Bool())
            .RuleFor(o => o.CostOfUnit, f => f.Random.Decimal(100, 10000))
            .RuleFor(o => o.Quantity, f => f.Random.Decimal(1, 100))
            .RuleFor(o => o.IsArchived, f => f.Random.Bool())
            .RuleFor(o => o.Created, f => f.Date.Past())
            .RuleFor(o => o.OfferDescriptionId, (f, o) =>
            {
                var OfferDescription = offerDescriptionFaker.Generate(1)[0];
                OfferDescriptions.Add(OfferDescription);
                return OfferDescription.Id;
            })
            .RuleFor(o => o.PlaceId, (f, o) =>
            {
                var place = placeFaker.Generate(1)[0];
                place.OfferId = o.Id;
                Places.Add(place);
                return place.Id;
            });

        var organizationFaker = new Faker<Organization>("ru")
            .RuleFor(o => o.OGRN, f => f.Random.Long(100000000000, 999999999999).ToString())
            .RuleFor(o => o.INN, f => f.Random.Long(1000000000, 9999999999).ToString())
            .RuleFor(o => o.Name, f => f.Company.CompanyName(0))
            .RuleFor(o => o.Created, f => f.Date.Past())
            .RuleFor(o => o.Id, f => f.Random.Guid())
            .RuleFor(o => o.OffersId, (f, o) =>
            {
                var offers = offerFaker.GenerateBetween(10, 20);
                foreach (var offer in offers)
                {
                    offer.OrganizationId = o.Id;
                }
                Offers.AddRange(offers);
                return offers.Select(of => of.Id).ToList();
            })
            .RuleFor(o => o.PlacesId, (f, o) =>
            {
                var places = placeFaker.Generate(3);
                foreach (var place in places)
                {
                    place.OrganizationId = o.Id;
                }
                places[0].Title = "Юридический адрес";
                places[1].Title = "Фактический адрес";
                places[2].Title = "Почтовый адрес";
                Places.AddRange(places);
                return places.Select(p => p.Id).ToList();
            });

        var userWithRoleFaker = new Faker<UserWithRole>("ru")
            .RuleFor(uwr => uwr.Id, f => f.Random.Guid())
            .RuleFor(uwr => uwr.Role, f => f.PickRandom<UserAbility>())
            .RuleFor(uwr => uwr.OrganizationId, (f, uwr) =>
            {
                var organization = organizationFaker.Generate(1)[0];
                organization.UsersId = new List<Guid>
                {
                    uwr.Id
                };
                Organizations.Add(organization);
                return organization.Id;
            });

        var userFaker = new Faker<User>("ru")
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.MiddleName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName());

        var alice = userFaker.Generate(1)[0];
        alice.Id = Guid.Parse("fe8aef09-9683-4193-911b-dcad5f8323cc");

        var usersWithRole = userWithRoleFaker.GenerateBetween(5, 10);
        foreach (var userWithRole in usersWithRole)
        {
            userWithRole.UserId = alice.Id;
        }
        UsersWithRole.AddRange(usersWithRole);

        var bob = userFaker.Generate(1)[0];
        bob.Id = Guid.Parse("54307ca0-0663-48be-874e-e729ff35fd97");

        usersWithRole = userWithRoleFaker.GenerateBetween(5, 10);
        foreach (var userWithRole in usersWithRole)
        {
            userWithRole.UserId = bob.Id;
        }
        UsersWithRole.AddRange(usersWithRole);

        Users.Add(alice);
        Users.Add(bob);
    }
}