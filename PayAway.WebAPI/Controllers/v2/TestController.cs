using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PayAway.WebAPI.Controllers.v2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">The logger.</param>
        public TestController(ILogger<TestController> logger)
        {
            this.logger = logger;
        }


        /// <summary>Gets the build information.</summary>
        /// <returns>ActionResult&lt;List&lt;System.String&gt;&gt;.</returns>
        /// <remarks>
        ///    You can use this to find info about the deployed build when reporting bugs or enhancement requests
        /// </remarks>
        [HttpGet]
        [Route("buildinfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public ActionResult<List<string>> GetBuildInfo()
        {
            var assemblies = new List<Assembly>();

            assemblies.Add(Assembly.GetExecutingAssembly());

            foreach (var dependencyName in typeof(Program).Assembly.GetReferencedAssemblies())
            {
                try
                {
                    //if (dependencyName.ToString().ToUpper().StartsWith(@"PRESTOPAY"))
                    //{
                        // Try to load the referenced assembly...
                        assemblies.Add(Assembly.Load(dependencyName));
                    //}
                }
                catch
                {
                    // Failed to load assembly. Skip it.
                }
            }

            var versionList = assemblies.Select(a => $"{a.GetName().Name} - {GetVersion(a)}").OrderBy(value => value); ;

            return Ok(versionList.ToList());
        }

        #region === Utility Methods =========================================
        /// <summary>
        /// Returns the version for a specfied assy
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.String.</returns>
        private static string GetVersion(Assembly assembly)
        {
            string compileTimestamp = string.Empty; 

            try
            {
                compileTimestamp = assembly
                                        .GetCustomAttributes<AssemblyMetadataAttribute>()
                                        .First(a => a.Key == "CompileTimestamp")
                                        .Value;
            }
            catch
            {
                // swallow exception
            }

            return $"Compiled: [{compileTimestamp}], Assy File Version: [{assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version}]";
        }
        #endregion
    }
}
