/// <summary>
/// Class for holding time info
/// </summary>
public class CurrentTime
{
    private bool _isAM = false;
    private int _hours = 0;
    private int _minutes = 0;
    public CurrentTime()
    {
    }
    public CurrentTime(bool isAM, int hours, int minutes)
    {
        IsAM = isAM;
        Hours = hours;
        Minutes = minutes;
    }
    public bool IsAM { get => _isAM; private set => _isAM = value; }
    public int Hours { get => _hours; private set => _hours = value; }
    public int Minutes { get => _minutes; private set => _minutes = value; }

    public void IncreaseHours()
    {
        if (Hours == 11)
            Hours = 0;
        else
            Hours++;
    }
    public void IncreaseMinutes()
    {
        if (Minutes == 59){
            IncreaseHours();
            Minutes = 0;
        }
        else
            Minutes++;
    }
    public void DecreaseHours()
    {
        if (Hours == 0)
            Hours = 11;
        else
            Hours--;
    }
    public void DecreaseMinutes()
    {
        if (Minutes == 0){
            DecreaseHours();
            Minutes = 59;
        }
        else
            Minutes--;
    }
    public void ChangeAmPm()
    {
        IsAM = !IsAM;
    }

    public override bool Equals(object obj)
    {
        return obj is CurrentTime time &&
               _isAM == time._isAM &&
               _hours == time._hours &&
               _minutes == time._minutes &&
               IsAM == time.IsAM &&
               Hours == time.Hours &&
               Minutes == time.Minutes;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

