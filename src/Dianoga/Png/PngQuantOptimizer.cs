using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using nQuant;

namespace Dianoga.Png
{
	// uses PngOptimizer to crunch PNGs - this is the fastest optimizer by far, and quite effective: http://psydk.org/pngoptimizer
	public class PngQuantOptimizer : IImageOptimizer
	{
		private readonly Stream _pngStream;

        public PngQuantOptimizer(Stream pngStream)
		{
			_pngStream = pngStream;
		}

		public IOptimizerResult Optimize()
		{
            var quantizer = new WuQuantizer();

            using (var memoryStream = new MemoryStream())
            {

                _pngStream.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
             

                using (var bitmap = new Bitmap(_pngStream))
                {
                    using (var quantized = quantizer.QuantizeImage(bitmap))
                    {
                        quantized.Save(memoryStream, ImageFormat.Png);
                    }

                    var resultBytes = memoryStream.ToArray();

                    var result = new PngOptimizerResult();
                    result.Success = true;
                    result.SizeBefore = imageBytes.Length;                    
                    result.SizeAfter = resultBytes.Length;
                    result.OptimizedBytes = resultBytes;

                    return result;
                }               
            }        		
		}
	}
}
