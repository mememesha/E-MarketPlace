SELECT
    offer."Id",
    od."Title",
    c."Name" as "Category",
    offer."CostOfUnit",
    od."Image",
    offer."OrganizationId"
FROM "Offers" offer
    LEFT JOIN "OfferDescriptions" od ON offer."OfferDescriptionId" = od."Id"
    LEFT JOIN "Categories" c ON od."CategoryId" = c."Id"