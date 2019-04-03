<Query Kind="Program">
  <NuGetReference>CsvHelper</NuGetReference>
  <Namespace>CsvHelper</Namespace>
  <Namespace>CsvHelper.Configuration</Namespace>
  <Namespace>CsvHelper.TypeConversion</Namespace>
</Query>

void Main()
{
	var filePath = @"C:\Users\David\Desktop\Projects\PDB\RecommendationDefaults4.csv";
	var records = new List<RecommendationDefault>();
	
	using (TextReader reader = new StreamReader(filePath))
	{
		using (var csvreader = new CsvReader(reader))
		{
			//if (null != map)
			//	csvreader.Configuration.RegisterClassMap(map);

			records = csvreader.GetRecords<RecommendationDefault>().ToList();
		}
	}
	
	//records.Dump();
	
	var results = new List<String>();
	results = records.Select (r => String.Format(insertformat,r.ID, r.Scope, r.Severity, r.Status, r.Section, r.Name, EscapeSqlValue(r.Description), EscapeSqlValue(r.Recommendation),r.BusinessHours, r.Default)).ToList();//.Dump();
	results.Aggregate ((x1,x2)=>String.Format("{0}\r\n\r\n{1}",x1,x2)).Dump();
}


public String EscapeSqlValue(String value){
	return value.Replace("\'","\'\'");
}

public const String insertformat = @"	INSERT INTO [eddsqos].[eddsdbo].[EnvironmentCheckRecommendationsDefaults]
		([ID],[Scope],[Severity],[Status],[Section],[Name],[Description],[Recommendation],[BusinessHours],[DefaultValue])
	VALUES
		('{0}','{1}',{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}')";

// Define other methods and classes here
public class RecommendationDefault{
	public String ID{ get; set; }
	public String Section{ get; set; }
	public String Name{ get; set; }
	public String Scope{ get; set; }
	//public Int32 Severity{ get; set; }
	public String Severity{ get; set; }
	public String BusinessHours{ get; set; }
	public String Status{ get; set; }
	public String Description{ get; set; }
	public String Recommendation{ get; set; }
	public String Default{ get; set; }
}