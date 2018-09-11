using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Label;

namespace CcNet.Labeller {
    [ReflectorType("incrementalLabeller")]
    public class IncrementalLabeller : LabellerBase, ILabeller {

        /// <summary>
        /// Major number component of the version. 
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>0</default>
        [ReflectorProperty("major", Required = false)]
        public int Major { get; set; }

        /// <summary>
        /// Minor number component of the version. 
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>0</default>
        [ReflectorProperty("minor", Required = false)]
        public int Minor { get; set; }

        /// <summary>
        /// Build number component of the version. If not specified the build number is incremented on every successful integration. 
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>0</default>
        [ReflectorProperty("build", Required = false)]
        public int Build { get; set; }

        /// <summary>
        /// Specifies an optional revision increment value, if omitted, the revision will increment by one
        /// </summary>
        /// <default>0</default>
        [ReflectorProperty("revisionIncrement", Required = false)]
        public int RevisionIncrement { get; set; }

        /// <summary>
        /// Whether to increase the build number component if the integration fails. By default the build number component will only increase
        /// if the integration was successful.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("incrementOnFailure", Required = false)]
        public bool IncrementOnFailure { get; set; }

        /// <summary>
		/// Gets or sets a value indicating whether to reset revision number after a version change.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the revision number should be reset after a version change; otherwise, <c>false</c>.
		/// </value>
        /// <default>false</default>
		[ReflectorProperty("resetRevisionAfterVersionChange", Required = false)]
		public bool ResetRevisionAfterVersionChange { get; set; }

        public override string Generate(IIntegrationResult integrationResult) {

            Version oldVersion;

            // Try getting old version.
            try {
                Log.Debug(string.Concat("[incrementalLabeller] Old build label is: ", integrationResult.LastIntegration.Label));
                oldVersion = new Version(integrationResult.LastIntegration.Label);
            } catch (Exception) {
                oldVersion = new Version(Major, Minor, Build, 0);
            }

            Log.Debug(string.Concat("[incrementalLabeller] Old version is: ", oldVersion.ToString()));

            int currentMajor = (Major > 0) ? Major : oldVersion.Major;
            int currentMinor = (Minor > 0) ? Minor : oldVersion.Minor;
            int currentBuild = (Build > 0) ? Build : oldVersion.Build;
            int currentRevision = (RevisionIncrement > 0) ? oldVersion.Revision + RevisionIncrement : oldVersion.Revision + 1;

            // If the last build failed and increment on failure is false, reset current revision to old revision.
            if (integrationResult.LastIntegrationStatus != IntegrationStatus.Success && !IncrementOnFailure) {
                Log.Debug("[incrementalLabeller] Last build in failure state, revision will not be incremented");
                currentRevision = oldVersion.Revision;
            }

            // If the version has changed, figure out what to do to the revision number.
            if(currentMajor != oldVersion.Major || currentMinor != oldVersion.Minor || currentBuild != oldVersion.Build){
                Log.Debug("[incrementalLabeller] Version change detected");

                if(ResetRevisionAfterVersionChange){
                    currentRevision = 0;
                }
            }

            var newVersion = new Version(currentMajor,currentMinor, currentBuild, currentRevision);

            Log.Debug("[incrementalLabeller] New Version is: {0}", newVersion);
            
            return newVersion.ToString();
        }
    }
}