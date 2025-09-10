# SRecordMerge (S19 Merge)

A lightweight WPF utility to load, inspect, sort, and merge Motorola S‑Record (S0/S1/S2/S3/S7/S8/S9) files. It provides a simple UI to:
- Load multiple .s19/.srec/.sre files at once
- Extract records and sort them by address
- Preserve the first S0 (header) and the last end record (S7/S8/S9) if present in specific inputs
- Inspect parsed records in a table (Type, Address, Data Length, Data)
- Export the merged result to a new S‑Record file

## Screenshots
- UI shows two buttons (Load S-Record File, Export) and a list of parsed rows.
- You can add screenshots here (e.g., docs/screenshot-1.png).

## Features
- Multi-file import using file dialog
- S0 header record and last end record (S7/S8/S9) handling
- Address-aware sorting of S1/S2/S3 records
- Clean display of records in a GridView
- Export merged lines to a file via Save File dialog

## How it works (high level)
- Load: Select multiple S-Record files. The app will parse supported record types.
- Merge: Non-header/end records (S1/S2/S3) are collected and sorted by their numeric address.
- Header/End: If a file name contains "Boot_HSM_App.sre", the first S0 and the last end record (S7/S8/S9) from that file are used as header and footer in the final output.
- Export: The final ordered lines are written to a new file you choose.

## Tech Stack
- .NET 7
- WPF (MVVM pattern)
- CommunityToolkit.Mvvm (ObservableObject, RelayCommand)

## Project Structure
- App.xaml, MainWindow.xaml(.cs): WPF entry and host window
- S19MergeView/
  - S19MergeView.xaml: View (buttons + ListView)
  - S19MergeViewModel.cs: VM with commands for Load/Export, parsing/merging logic
  - S19MergeModel.cs: SRecord model (Type, Address, DataLen, DataString)
- Services/BindableBase.cs: Convenience base class for bindable properties built on ObservableObject

## Requirements
- Visual Studio 2022 (or newer)
- .NET 7 SDK
- Windows (WPF)

## Getting Started
1. Clone the repository
   - git clone <your-repo-url>
2. Open S19Merge.sln in Visual Studio 2022.
3. Restore NuGet packages if prompted (CommunityToolkit.Mvvm).
4. Set S19Merge as startup project and press F5 to run.

## Usage
1. Click "Load S-Record File" and select one or more S-Record files (.s19/.srec/.sre).
2. The list will show parsed records (Type, Address, Data Length, Data).
3. Records will be sorted by address (S1, S2, S3). If a file named like "Boot_HSM_App.sre" is included, its S0 header and the last S7/S8/S9 record are used.
4. Click "Export" to save the merged S-Record as a new file.

## Notes & Limitations
- Parsing logic targets standard S0/S1/S2/S3/S7/S8/S9 lines. Lines outside these types are ignored.
- The special handling for header/end records is keyed on file names that contain "Boot_HSM_App.sre" (case-insensitive). Adjust logic in S19MergeViewModel if your naming differs.
- The tool does not currently validate checksums or rewrite them.

## Roadmap (ideas)
- Configurable header/end selection rules
- Checksum verification and regeneration
- Command-line mode
- Unit tests for parser and merger

## Contributing
- Issues and PRs are welcome. Please keep changes focused and include a brief description and testing notes.

## License
- Add your preferred license (e.g., MIT) and license file if applicable.
