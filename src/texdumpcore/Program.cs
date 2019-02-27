using System;
using System.IO;

using CommandLine;
using CommandLine.Text;
using CTR;

using SixLabors.ImageSharp;

namespace texdump
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options)) { return; }
            if (options.LastParserState != null)
                if (options.LastParserState.Errors.Count > 0) { return; }

            if (!File.Exists(options.Input))
            {
                Console.WriteLine($"{options.Input} doesn't exists.");
                return;
            }

            CTRTexture texture = new CTRTexture();
            texture.Size = options.Size;
            texture.Format = options.Format;

            var fs = File.OpenRead(options.Input);
            BinaryReader reader = new BinaryReader(fs);
            fs.Position = options.Position;
            if (options.Length > 0)
            {
                texture.ImageData = reader.ReadBytes(options.Length);
            }
            else
            {
                var length = (int)(options.Width * options.Height * TextureUtil.GetBytesPerPixel(options.Format));
                texture.ImageData = reader.ReadBytes(length);
            }

            var bmp = TextureUtil.DecodeTexture(texture);

            if (!string.IsNullOrEmpty(options.Output))
                bmp.Save(options.Output);
        }
    }

    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Set input file.")]
        public string Input { get; set; }
        [Option('o', "output", HelpText = "Set output file.")]
        public string Output { get; set; }
        [Option('f', "format", Required = true, HelpText = "Set pixel format.")]
        public CTR.TextureFormat Format { get; set; }
        [Option('p', "position", DefaultValue = 0, HelpText = "Set texture data position.")]
        public int Position { get; set; }
        [Option('l', "length", HelpText = "Set texture data length.")]
        public int Length { get; set; }
        [Option('w', "width", HelpText = "set image width.")]
        public int Width { get; set; }
        [Option('h', "height", HelpText = "Set image height.")]
        public int Height { get; set; }

        public Size Size { get { return new Size(Width, Height); } }

        [ParserState]
        public IParserState LastParserState { get; set; }
        [HelpOption("help")]
        public string GetHelp()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
