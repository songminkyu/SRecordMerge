using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using S19Merge.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace S19Merge.ViewModel
{
    public class S19MergeViewModel : S19MergeViewModelBase
    {
        public S19MergeViewModel() 
        {
            allAddresses         = new List<string>();
            SRecords             = new ObservableCollection<SRecord>();
            SRecordLoadedCommand = new RelayCommand<object>(SRecordLoadedCommandExe);
            SRecordExportCommand = new RelayCommand<object>(SRecordExportCommandExe);
        }

        private void SRecordExportCommandExe(object? obj)
        {
            if(allAddresses != null && allAddresses.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllLines(saveFileDialog.FileName, allAddresses);
                }
            }            
        }
        private void SRecordLoadedCommandExe(object? obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true && allAddresses != null && SRecords != null)
            {
                allAddresses.Clear();
                SRecords.Clear();

                List<string> mainAddress  = new List<string>();
                List<SRecord> mainRecords = new List<SRecord>();
                string ?s0Record          = null;
                string ?lastEndRecord     = null;

                foreach (var file in openFileDialog.FileNames)
                {
                    var lines = File.ReadAllLines(file);

                    //특정 파일에서 S0/S7,S8,S9 발취
                    if (file.Contains("Boot_HSM_App.sre", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        s0Record       = lines.FirstOrDefault(line => line.StartsWith("S0"));
                        var endRecords = lines.Where(line => line.StartsWith("S7") || line.StartsWith("S8") || line.StartsWith("S9")).ToList();
                        lastEndRecord  = endRecords.LastOrDefault();
                    }

                    foreach (var line in lines)
                    {
                        var record = ExtractRecordsFromSRecord(line);
                        if (record == null) continue;

                        if (record.Type != "S0" && record.Type != "S7" && record.Type != "S8" && record.Type != "S9")
                        {
                            mainAddress.Add(line);
                            mainRecords.Add(record);
                        }
                    }
                }

                // mainRecords (S1, S2, S3)와 mainAddress를 주소별로 오름차순 정렬
                mainRecords = mainRecords.OrderBy(r => Convert.ToInt64(r.Address, 16)).ToList();
                mainAddress = mainAddress.OrderBy(s =>
                {
                    int length = s.StartsWith("S3") ? 8 :
                                 s.StartsWith("S2") ? 6 :
                                 s.StartsWith("S1") ? 4 : 0;
                    return Convert.ToInt64(s.Substring(4, length), 16);
                }).ToList();

                // 최종 allAddresses 구성
                if (s0Record != null)
                {
                    allAddresses.Add(s0Record);
                }
                allAddresses.AddRange(mainAddress);

                if (lastEndRecord != null)
                {
                    allAddresses.Add(lastEndRecord);
                }

                // SRecords에 각 항목 추가
                if (s0Record != null)
                {
                    SRecords.Add(ExtractRecordsFromSRecord(s0Record)!);
                }
                foreach (var record in mainRecords)
                {
                    SRecords.Add(record);
                }
                if (lastEndRecord != null)
                {
                    SRecords.Add(ExtractRecordsFromSRecord(lastEndRecord)!);
                }
            }
        }

        private SRecord? ExtractRecordsFromSRecord(string line)
        {
            if (line.StartsWith("S0") || line.StartsWith("S1") || line.StartsWith("S2") || line.StartsWith("S3") ||
                line.StartsWith("S7") || line.StartsWith("S8") || line.StartsWith("S9"))
            {
                int addressLength = line switch
                {
                    { } s when s.StartsWith("S1") => 2,
                    { } s when s.StartsWith("S2") => 3,
                    { } s when s.StartsWith("S3") => 4,
                    { } s when s.StartsWith("S0") => 2,
                    { } s when s.StartsWith("S7") => 4,
                    { } s when s.StartsWith("S8") => 3,
                    { } s when s.StartsWith("S9") => 2,
                    _ => 0
                };

                string type    = line.Substring(0, 2);
                int count      = Convert.ToInt32(line.Substring(2, 2), 16);
                string address = line.Substring(4, addressLength * 2);
                int dataLength = (count - addressLength - 1) * 2;  // Minus address and checksum bytes
                string data    = line.Substring(4 + addressLength * 2, dataLength);

                return new SRecord()
                {
                    Type       = type,
                    Address    = address,
                    DataLen    = (data.Length / 2).ToString(),
                    DataString = InsertSpaces(data)
                };
            }
            return null;
        }

        private string InsertSpaces(string input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i += 2)
            {
                sb.Append(input.Substring(i, 2));
                if (i + 2 < input.Length)
                    sb.Append(" ");
            }
            return sb.ToString();
        }       
    }
}
