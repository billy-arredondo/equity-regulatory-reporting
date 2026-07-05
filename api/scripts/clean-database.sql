-- Deletes all business-logic data while leaving authentication/Identity
-- tables (AspNetUsers, AspNetRoles, refresh_tokens, etc.), the schema,
-- and migration history untouched. Safe to run multiple times.
--
-- Usage (psql):
--   psql -h localhost -U postgres -d equity_regulatory_reporting -f scripts/clean-database.sql
--
-- Usage (docker exec):
--   docker exec -i <container> psql -U postgres -d equity_regulatory_reporting \
--     -f /scripts/clean-database.sql

TRUNCATE
    -- Transactional / user-generated data (children first)
    board_members,
    participations,

    -- Core domain (self-referential persons last within this group;
    -- CASCADE handles the representative_id self-FK automatically)
    persons,
    document_type_person_types,
    document_types,
    countries,
    locations,
    positions

CASCADE;
