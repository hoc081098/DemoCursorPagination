namespace DemoCursorPagination.Contracts;

public sealed record NoteResponse(
    Guid Id,
    Guid UserId,
    string Note,
    DateOnly NoteDate
);

public static class UsersNotesExtensions
{
    public static NoteResponse ToResponse(this Models.UserNotes userNotes) =>
        new(
            Id: userNotes.Id,
            UserId: userNotes.UserId,
            Note: userNotes.Note,
            NoteDate: userNotes.NoteDate
        );
}