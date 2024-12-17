SELECT DISTINCT s."Id", u."UserId", s."Url", s."Method"
FROM "KeyAllowedEndpoints" a
JOIN "UserApiKeys" u ON a."UserApiKeyId" = u."Id"
JOIN "ServiceEndpoints" s ON a."EndpointId" = s."Id"
WHERE u."UserId" = 42;

SELECT DISTINCT u."UserId"
FROM "KeyAllowedEndpoints" k
JOIN "UserApiKeys" u on k."UserApiKeyId" = u."Id"
JOIN "ServiceEndpoints" s on k."EndpointId" = s."Id"
WHERE s."Url" like '/api/Emails' and s."Method" like 'Post';

select * from "Users"
select * from "ServiceEndpoints"
select * from "KeyAllowedEndpoints"

select * from "UserApiKeys"