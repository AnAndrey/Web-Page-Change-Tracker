# Web-Page-Change-Tracker
Util to to monitor particulcar changes on a web pages and notify about them. Pre-employment testing practice in Genes1s, Russian IT company. 

## How it works
1. Get data1 from first source
2. Get data2 from second source
3. Compare data1 with data2
4. If changes are detected:
  - Notify about changes
  - Save data1 as data2 for next comparing cycle
  
## More details
- Data model presents in a tree-like structure. Regions are ont the top. Every region has a district and every district is a set of locations.
- Data1 retrieves from web pages. Retrieved information is processing by HTML parcers which are linked into "Chain of responsibility". The processing performs in parallel threads. The count of working thread is customizable, 20 is default. Parcers are raising events about activity and have ability of cancellation in case of hangs.
- Data2 is a presaved info. LocalDB is used as data storage. You can choose another DB which supports by Entity Framework 6. Also you can implement custom provider and save data to any kind of storage.
- All found changes are sent by E-mail. SMTP settings and e-mal message fields are customizable and can be set in configuration file. Also you can implement custom informer.
- log4net is used as logging tool.
- CodeContracts performs parameters validation.



