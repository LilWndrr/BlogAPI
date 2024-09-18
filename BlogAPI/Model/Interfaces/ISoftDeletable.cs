namespace BlogAPI.Model.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}