// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");



public class Instrument
{

}
public class Stock:Instrument
{
    public virtual string Volatility { get; set; } = "High";
}

public class SNP500IndexFund : Stock
{

}

public class EmergingMarketFund : Stock
{
    public override string Volatility { get=>base.Volatility; set=>base.Volatility=value; }
}
