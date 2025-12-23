CREATE SCHEMA IF NOT EXISTS identity;
CREATE SCHEMA IF NOT EXISTS notes;

DROP TABLE IF EXISTS "identity"."users";
-- Table Definition
CREATE TABLE "identity"."users" (
    "id" uuid NOT NULL DEFAULT uuidv7(),
    "name" varchar(20) NOT NULL,
    PRIMARY KEY ("id")
);

DROP TABLE IF EXISTS "notes"."user_notes";
-- Table Definition
CREATE TABLE "notes"."user_notes" (
    "id" uuid NOT NULL DEFAULT uuidv7(),
    "user_id" uuid NOT NULL,
    "note" varchar(500) NOT NULL,
    "note_date" date NOT NULL DEFAULT CURRENT_DATE,
    PRIMARY KEY ("id")
);

ALTER TABLE notes.user_notes
  ADD CONSTRAINT fk_user_notes__user
  FOREIGN KEY (user_id) REFERENCES identity.users(id)
  ON DELETE CASCADE;

-- Indices
CREATE INDEX IF NOT EXISTS idx_user_notes_user_id
  ON notes.user_notes (user_id);

CREATE INDEX IF NOT EXISTS idx_user_notes_note_date_id
  ON notes.user_notes (note_date DESC, id DESC);