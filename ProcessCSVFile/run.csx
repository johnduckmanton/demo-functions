#r "System.IO"
#r "System.Text.RegularExpressions"

using CsvHelper;
using System;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Process a csv file containing the details of Titanic survivors.
/// 
/// The passenger name column (pas_name) is expected to be in the format "surname, title forenames"
/// This function will to reformat this to "title forenames surname" and store the result in a new column (pas_name2).
/// 
/// If any errors are found in the pas_name format a 1 will be placed in a new column 
/// called "name_format_error" on corresponding row
/// </summary>
/// <param name="myBlob">the input blob</param>
/// <param name="name">The name of the input file</param>
/// <param name="outputBlob">The blob that the processed csv file will be written to</param>
/// <param name="log">TraceWriter instance for logging</param>
public static void Run(Stream myBlob, string name, Stream outputBlob, TraceWriter log)
{
    log.Info($"C# Blob trigger function Processed blob\n Name:{name}.csv \n Size: {myBlob.Length} Bytes");

    var _record = new TitanicSurvivor();
    int _recordsRead = 0;
    int _recordsWritten = 0;
    int _recordsInError = 0;

    try
    {
        using (StreamReader sr = new StreamReader(myBlob))
        {

            CsvReader csvReader = new CsvReader(sr);

            // Configure the CsvReader to ignore missing fields. This is a bit of a hack that allows us to use 
            // the same record format for both the input file and output file. Since we're adding 
            // two new columns to the output file we need to tell CsvReader to ignore errors 
            //due to not finding these columns in the input file.
            csvReader.Configuration.MissingFieldFound = null;

            // Read the input file header
            csvReader.Read();
            csvReader.ReadHeader();

            // Write out a corresponding header to a new output file 
            TextWriter tr = new StreamWriter(outputBlob);
            CsvWriter csvWriter = new CsvWriter(tr);
            csvWriter.WriteHeader<TitanicSurvivor>();
            csvWriter.NextRecord();

            // Iterate through the input file rows, process each one and write the processed 
            // output to the output file
            while (csvReader.Read())
            {
                _recordsRead++;
                _record = csvReader.GetRecord<TitanicSurvivor>();
                _record.name_format_error = 0;

                // Here we're using a simple regex to check for the name pattern we expect
                // If we don't find it we flag the row as an error row
                Regex regex = new Regex(@"^([A-Za-z]*),?\s*([A-Za-z\s,]*)$");
                Match match = regex.Match(_record.pas_name);
                if (!match.Success)
                {
                    _record.pas_name2 = "** pas_name contains invalid characters. Must be alphabetic characters, comma or SPC only **";
                    _record.name_format_error = 1;
                    _recordsInError++;
                }
                else
                {
                    // The names should be in the format <surname>, <forenames>, <suffix> e.g. Thomas, Mr John, Jr
                    // We're going to do some simple string manipulation to create a new name in the format
                    // <forenames> <surname> <suffix> e.g. Mr John Thomas Jr
                    String[] tempStr = _record.pas_name.Split(',');
                    if (tempStr.Length == 2)
                    {
                        _record.pas_name2 = string.Format("{0} {1}", tempStr[1]?.Trim(), tempStr[0]?.Trim());
                    }
                    else if (tempStr.Length == 3)
                    {
                        _record.pas_name2 = string.Format("{0} {1} {2}", tempStr[1]?.Trim(), tempStr[0]?.Trim(), tempStr[2]?.Trim());
                    } 
                    else
                    {
                        _record.pas_name2 = "** pas_name is incorrectly formatted. Should be <surname>, <forenames>, [<suffix>]";
                        _record.name_format_error = 1;
                    }
                }

                csvWriter.WriteRecord(_record);
                csvWriter.NextRecord();

                _recordsWritten++;
            }

            log.Info(string.Format("{0} records read. {1} records processed. {2} errors", _recordsRead, _recordsWritten, _recordsInError));

            tr.Flush();
            tr.Close();
            tr.Dispose();

        }
    }
    catch (Exception ex)
    {
        log.Error(string.Format("Error Occurred. {0}", ex.ToString()));
    }
}


/// <summary>
/// POCO representing a row in the csv file with the following format:
///
/// Id,pas_name,class,pas_age,sex_name,is_survive sex_code
/// 1,Allen,Miss Elisabeth Walton,1st,29,female,1,1
///
/// </summary>
class TitanicSurvivor
{
    public string Id { get; set; }
    public string pas_name { get; set; }
    public string @class { get; set; }
    public string pas_age { get; set; }
    public string sex_name { get; set; }
    public int is_survive { get; set; }
    public int sex_code { get; set; }

    // These columns will be added to the output file
    public string pas_name2 { get; set; }
    public int name_format_error { get; set; }

}
