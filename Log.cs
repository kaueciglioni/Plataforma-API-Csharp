using System;
using System.IO;
using VoxLogger;

namespace WindowsFormsApp
{
    /// <summary>
    /// Classe modelo para a criação e controle do Log.
    /// Toda vez que for utilizar mais de um arquivo de log por projeto, deve ser criado um log por projeto.
    /// </summary>
    public class Log
    {
        #region Constant
        /// <summary>
        /// Nome do projeto
        /// Deve ser atribuido no construtor
        /// </summary>
        private const string ProjectName = "WindowsFormsApp";
        #endregion

        #region Constructors / Finalizer
        /// <summary>
        /// Utilizado somente para criação da classe
        /// </summary>
        /// <param name="p_configLogfileName"></param>
        private Log(string p_configLogfileName)
        {
            try
            {
                Logger = new Logger(p_configLogfileName, ProjectName);
            }
            catch (Exception l_exception)
            {
                Console.WriteLine(l_exception.ToString());
                throw;
            }
        }
        #endregion

        #region Atributes
        /// <summary>
        /// Objeto Logger Private
        /// </summary>
        private static Logger m_logger;

        /// <summary>
        /// Instancia do objeto Singleton
        /// </summary>
        private static Log m_instance;

        /// <summary>
        /// Lock responsável para controle de concorrência na construção do objeto
        /// </summary>
        private static readonly object m_lock = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Controlador do objeto Logger
        /// </summary>
        public static Logger Logger
        {
            get
            {
                try
                {
                    if (m_logger == null)
                    {
                        string l_configLogfileName = VoxConfigurationManager.VoxConfiguration.GetParameter("configFileLogPath").ToString();
                        //string l_configLogfileName = "";

                        //Verificar se o Nó configFilelogPath está no App.VoxConfig
                        if (string.IsNullOrEmpty(l_configLogfileName))
                        {
                            CreateInstance(null);
                        }
                        else
                        {
                            string l_directoryLogName;

                            try
                            {
                                l_directoryLogName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                            }
                            catch
                            {
                                l_directoryLogName = "";
                            }

                            if (string.IsNullOrEmpty(l_directoryLogName))
                            {
                                l_directoryLogName = AppDomain.CurrentDomain.BaseDirectory;
                            }
                            //else
                            //{
                            //    l_directoryLogName = Path.GetDirectoryName(l_directoryLogName);
                            //}

                            if (l_directoryLogName != null)
                            {
                                CreateInstance(l_directoryLogName.TrimEnd('\\') + "\\" + l_configLogfileName);
                            }
                        }
                    }
                    return m_logger;
                }
                catch (Exception l_exception)
                {
                    throw;
                }
            }
            private set { m_logger = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retorna a Instancia já criada ou cria
        /// </summary>
        /// <param name="p_configLogfileName">Caminho do arquivo de configuração do Log</param>
        /// <returns>Instancia da classe Singleton</returns>
        private static Log CreateInstance(string p_configLogfileName)
        {
            try
            {
                lock (m_lock)
                {
                    if (m_instance == null)
                    {
                        m_instance = new Log(p_configLogfileName);
                    }
                }
                return m_instance;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}