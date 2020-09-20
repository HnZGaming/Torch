using System;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using NLog;
using Torch.API;
using Torch.Managers;

namespace Torch.Server.Firebase
{
    /// <summary>
    /// Initializes Firebase integration.
    /// </summary>
    public class FirebaseManager : Manager
    {
        static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc/>
        public FirebaseManager(ITorchBase torchInstance) : base(torchInstance)
        {
        }

        /// <inheritdoc/>
        public override void Attach()
        {
            try
            {
                var config = FirebaseConfig.Load();
                var credentialPath = config.GoogleCredentialJsonPath;
                var credential = GoogleCredential.FromFile(credentialPath);
                FirebaseApp.Create(new AppOptions
                {
                    Credential = credential,
                });

                _logger.Info("Initialized Firebase");
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        /// <inheritdoc/>
        public override void Detach()
        {
        }
    }
}