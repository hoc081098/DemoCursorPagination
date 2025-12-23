-- =============================================
--  Seed Data
-- =============================================

-- 1) 1,000 Users
INSERT INTO identity.users (id, name)
SELECT uuidv7(), 'user_' || gs
FROM generate_series(1, 1000) AS gs;

-- 2) 1,000,000 Notes (randomized)
DO $$
DECLARE
  v_max_rn int;
BEGIN
  -- Map each user with row number for fast random access
  CREATE TEMP TABLE tmp_users_rn AS
  SELECT row_number() OVER ()::int AS rn, id
  FROM identity.users;
  CREATE UNIQUE INDEX ON tmp_users_rn (rn);

  SELECT max(rn) INTO v_max_rn FROM tmp_users_rn;

  RAISE NOTICE 'Seeding 1,000,000 notes...';

  PERFORM set_config('synchronous_commit','off', true);

  INSERT INTO notes.user_notes (id, user_id, note, note_date)
  SELECT
    uuidv7(),
    (SELECT id FROM tmp_users_rn WHERE rn = 1 + floor(random() * v_max_rn)::int),
    left(md5(gs::text || ':' || clock_timestamp()::text), 80),
    (DATE '2020-01-01' + ((random() * (CURRENT_DATE - DATE '2020-01-01'))::int))::date
  FROM generate_series(1, 1000000) AS gs;

  PERFORM set_config('synchronous_commit','on', true);

  RAISE NOTICE 'Seeding completed.';
END$$;

-- Analyze after bulk insert for better query plans
ANALYZE identity.users;
ANALYZE notes.user_notes;

-- =============================================
--  Done.
-- =============================================