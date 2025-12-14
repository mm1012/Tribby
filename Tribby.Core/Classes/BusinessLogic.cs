using Tribby.Core.Enums;

public class BusinessLogic
{
    
    public Share ProcessShare(int shareTypeId, decimal totalAmount, int numberOfUsers)
    {
        switch (shareTypeId)
        {
            case (int)ShareTypes.Equal:
                decimal equalShare = totalAmount / numberOfUsers;
                // Logic for equal share
                break;
            case (int)ShareTypes.Exact:
                // Logic for exact share
                break;
            case (int)ShareTypes.Percentage:
                // Logic for percentage share
                break;
            case (int)ShareTypes.Shares:
                // Logic for shares
                break;
            case (int)ShareTypes.ExactAndSplit:
                // Logic for exact and split
                break;
            default:
                throw new ArgumentException("Invalid share type");
        }

        return new Share();
    }

}