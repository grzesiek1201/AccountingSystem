using AccountingSystem.Domain.Enums;

public class NumberSequence
{
    protected NumberSequence()
    {
    }

    public NumberSequence(
        DocumentType documentType,
        int year,
        int lastNumber = 1)
    {
        DocumentType = documentType;
        Year = year;
        LastNumber = lastNumber;
    }

    public int Id { get; set; }
    public DocumentType DocumentType { get; private set; }
    public int Year { get; private set; }
    public int LastNumber { get; private set; }

    public void Increment()
    {
        LastNumber++;
    }
}