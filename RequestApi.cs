using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp;

namespace PythonApi
{
    public class RequestApi
    {
        private static readonly HttpClient m_client = new HttpClient();
        public string l_local_arquivo = VoxConfigurationManager.VoxConfiguration.GetParameter("link").ToString();

        public async Task<string> CreateAudio(string p_guid, string p_audioFilePath)
        {
            string l_responseString = string.Empty;

            string l_fileName = Path.GetFileName(p_audioFilePath);
            string l_newFileName = $"{l_fileName}";

            using (var l_formData = new MultipartFormDataContent())
            {
                l_formData.Add(new StringContent(p_guid), "guid");

                var l_fileStream = File.OpenRead(p_audioFilePath);
                var l_streamContent = new StreamContent(l_fileStream);
                l_streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "audios",
                    FileName = l_newFileName
                };
                l_formData.Add(l_streamContent);

                try
                {
                    var l_response = await m_client.PostAsync("http://localhost:5000/Home/create", l_formData);
                    l_response.EnsureSuccessStatusCode();
                    l_responseString = await l_response.Content.ReadAsStringAsync();
                    Log.Logger.Debug("RequestApi", "CreateAudioAsync", "Requisição enviada com sucesso.");
                }
                catch (Exception l_ex)
                {
                    Log.Logger.Error("RequestApi", "CreateAudioAsync", l_ex);
                    throw new Exception("Erro ao enviar o arquivo: " + l_ex.Message);
                }
                finally
                {
                    l_fileStream.Close();
                    Log.Logger.Debug("RequestApi", "CreateAudioAsync", "GUID e AUDIOS CRIADAS COM SUCESSO");
                }
            }

            return l_responseString;
        }

        public async Task<string> DeleteAudio(string p_guid)
        {
            string l_responseString = string.Empty;

            try
            {
                var l_response = await m_client.DeleteAsync($"http://localhost:5000/Home/delete/{p_guid}");
                l_response.EnsureSuccessStatusCode();
                l_responseString = await l_response.Content.ReadAsStringAsync();
                Log.Logger.Debug("RequestApi", "DeleteAudio", "Arquivo deletado com sucesso");
            }
            catch (Exception l_ex)
            {
                Log.Logger.Error("RequestApi", "DeleteAudioAsync", l_ex);
                throw new Exception("Erro ao deletar o arquivo: " + l_ex.Message);
            }

            return l_responseString;
        }

        public async Task<string> UpdateAudio(string p_guid, string p_newGuid, string p_audioFilePath)
        {
            string l_responseString = string.Empty;

            using (var l_formData = new MultipartFormDataContent())
            {
                l_formData.Add(new StringContent(p_newGuid), "guid");

                using (OpenFileDialog l_openFileDialog = new OpenFileDialog())
                {
                    l_openFileDialog.Filter = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
                    if (l_openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        p_audioFilePath = l_openFileDialog.FileName;

                        string l_fileName = Path.GetFileName(p_audioFilePath);
                        string l_newFileName = $"{l_fileName}";

                        var l_fileStream = File.OpenRead(p_audioFilePath);
                        var l_streamContent = new StreamContent(l_fileStream);
                        l_streamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                        {
                            Name = "audios",
                            FileName = l_newFileName
                        };
                        l_formData.Add(l_streamContent);

                        try
                        {
                            var l_response = await m_client.PutAsync($"http://localhost:5000/Home/update/{p_guid}", l_formData);
                            l_response.EnsureSuccessStatusCode();
                            l_responseString = await l_response.Content.ReadAsStringAsync();
                            Log.Logger.Debug("RequestApi", "UpdateAudio", "Arquivo alterado com sucesso");

                            l_fileStream.Close();
                        }
                        catch (Exception l_ex)
                        {
                            Log.Logger.Error("RequestApi", "UpdateAudio", l_ex);
                            throw new Exception("Erro ao atualizar o arquivo: " + l_ex.Message);
                        }
                    }
                }
            }

            return l_responseString;
        }

        public async Task<string> GetAudio(string p_guid)
        {
            string l_downloadFilePath = string.Empty;

            try
            {
                var l_response = await m_client.GetAsync($"http://localhost:5000/Home/{p_guid}");
                l_response.EnsureSuccessStatusCode();

                var l_responseStream = await l_response.Content.ReadAsStreamAsync();

                string l_downloadsPath = l_local_arquivo;

                if (!Directory.Exists(l_downloadsPath))
                {
                    Directory.CreateDirectory(l_downloadsPath);
                }

                string l_fileName = $"{p_guid}_downloaded_audio.mp3";
                l_downloadFilePath = Path.Combine(l_downloadsPath, l_fileName);

                using (var l_fileStream = new FileStream(l_downloadFilePath, FileMode.Create, FileAccess.Write))
                {
                    await l_responseStream.CopyToAsync(l_fileStream);
                }

                Log.Logger.Debug("RequestApi", "GetAudio", "Áudio baixado com sucesso");
            }
            catch (Exception l_ex)
            {
                Log.Logger.Error("RequestApi", "GetAudio", l_ex);
                throw new Exception("Erro ao baixar o arquivo: " + l_ex.Message);
            }

            return l_downloadFilePath;
        }
    }
}
