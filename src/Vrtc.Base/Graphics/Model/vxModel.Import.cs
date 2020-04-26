using System.Collections.Generic;
using System.IO;


namespace VerticesEngine.Graphics
{
    public enum vxImportResultStatus
    {
        Success,
        Warnings,
        Errors
    }
    public class vxImportResult
    {
        public vxImportResultStatus ImportResultStatus;
        List<string> Errors = new List<string>();
        List<string> Warnings = new List<string>();

        public readonly vxModel ImportedModel;

        public vxImportResult()
        {
            ImportResultStatus = vxImportResultStatus.Errors;
        }

        public vxImportResult(vxModel model)
        {
            ImportedModel = model;
            ImportResultStatus = vxImportResultStatus.Success;
        }

        public vxImportResult(vxModel model, List<string> warnings)
        {
            ImportedModel = model;
            ImportResultStatus = vxImportResultStatus.Warnings;
            Warnings.AddRange(warnings);
        }

        public vxImportResult(List<string> errors)
        {
            ImportResultStatus = vxImportResultStatus.Errors;
            Errors.AddRange(errors);
        }
    }
    /// <summary>
    /// A Model Class which loads and processes all data at runtime. Although this add's to load times,
    /// it allows for more control as well as modding for any and all models which are used in the game.
    /// Using three different models to handle different types of rendering does add too over all installation
    /// size, it is necessary to allow the shaders to be compiled for cross platform use.
    /// </summary>
    public partial class vxModel : vxGameObject
    {
        public static vxImportResult Import(vxEngine Engine,string filepath)
        {
            FileInfo file = new FileInfo(filepath);
            vxImportResult result = new vxImportResult();
            switch (file.Extension)
            {
                case ".obj":
                    result = ImportOBJ(Engine, filepath);
                    break;
            }
            return result;
        }
    }
}

