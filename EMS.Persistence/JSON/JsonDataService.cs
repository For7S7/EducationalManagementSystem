using EMS.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Text;
using EMS.Persistence.JSON.DataTransferObjects;

namespace EMS.Persistence.JSON
{
    public class JsonDataService
    {
        private readonly DataStoreMapper _mapper;
        private readonly string _filePath;

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        public JsonDataService()
        {
            _mapper = new DataStoreMapper();
            string directory = Path.Combine(AppContext.BaseDirectory, "Data");
            Directory.CreateDirectory(directory);

            _filePath = Path.Combine(directory, "ems-data.json");
        }

        public void SaveData(IDataStore dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));

            DataStoreDTO dto = _mapper.ToDto(dataStore);

            string json = JsonSerializer.Serialize(dto, _options);

            File.WriteAllText(_filePath, json);
        }

        public void LoadData(IDataStore dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException(nameof(dataStore));

            if (!File.Exists(_filePath))
                return;

            string json = File.ReadAllText(_filePath);

            DataStoreDTO dto = JsonSerializer.Deserialize<DataStoreDTO>(json, _options);

            if (dto == null)
                throw new InvalidOperationException(
                    "Failed to deserialize JSON data.");

            _mapper.LoadFromDto(dto, dataStore);
        }
    }
}
