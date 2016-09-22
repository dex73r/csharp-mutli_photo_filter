using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace school_color_changer {

    public static class CBitmap {

        public static Bitmap copy_to_canvas ( this Bitmap sourceBpm, int canvasWidth )
        {
            var maxSide = sourceBpm.Width > sourceBpm.Height
                ? sourceBpm.Width
                : sourceBpm.Height;

            var rate = maxSide / ( float ) canvasWidth;

            var bmpResult = sourceBpm.Width > sourceBpm.Height
                ? new Bitmap ( canvasWidth, ( int ) ( sourceBpm.Height / rate ) )
                : new Bitmap ( ( int ) ( sourceBpm.Width / rate ), canvasWidth );

            using ( var gfxResult = Graphics.FromImage ( bmpResult ) ) {
                gfxResult.CompositingQuality = CompositingQuality.HighQuality;
                gfxResult.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfxResult.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfxResult.DrawImage (
                    sourceBpm,
                    new Rectangle (
                        0,
                        0,
                        bmpResult.Width,
                        bmpResult.Height ),
                    new Rectangle (
                        0,
                        0,
                        sourceBpm.Width,
                        sourceBpm.Height ),
                    GraphicsUnit.Pixel );
                gfxResult.Flush ( );
            }

            return bmpResult;
        }

        public static Bitmap color_tinter (
            this Bitmap srcBpm, float blueRatio,
            float greenRatio, float redRatio )
        {
            var bmpData = srcBpm.LockBits (
                new Rectangle (
                    0,
                    0,
                    srcBpm.Width,
                    srcBpm.Height ),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb );

            var buffPixels = new byte[bmpData.Stride * bmpData.Height];

            Marshal.Copy ( bmpData.Scan0, buffPixels, 0, buffPixels.Length );

            srcBpm.UnlockBits ( bmpData );

            for ( var k = 0; k + 4 < buffPixels.Length; k += 4 ) {
                var blue = buffPixels[k] + ( 255 - buffPixels[k] ) * blueRatio;
                var green = buffPixels[k + 1] + ( 255 - buffPixels[k + 1] ) * greenRatio;
                var red = buffPixels[k + 2] + ( 255 - buffPixels[k + 2] ) * redRatio;

                if ( blue > 255 )
                    blue = 255;

                if ( green > 255 )
                    green = 255;

                if ( red > 255 )
                    red = 255;

                buffPixels[k] = ( byte ) blue;
                buffPixels[k + 1] = ( byte ) green;
                buffPixels[k + 2] = ( byte ) red;
            }

            var bmpResult = new Bitmap ( srcBpm.Width, srcBpm.Height );

            var dataResult = bmpResult.LockBits (
                new Rectangle (
                    0,
                    0,
                    bmpResult.Width,
                    bmpResult.Height ),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb );

            Marshal.Copy ( buffPixels, 0, dataResult.Scan0, buffPixels.Length );
            bmpResult.UnlockBits ( dataResult );

            return bmpResult;
        }

        private static Bitmap grab_arb_copy ( Image sourceImage )
        {
            var bpm = new Bitmap ( sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb );

            using ( var graphics = Graphics.FromImage ( bpm ) ) {
                graphics.DrawImage (
                    sourceImage,
                    new Rectangle ( 0, 0, bpm.Width, bpm.Height ),
                    new Rectangle ( 0, 0, bpm.Width, bpm.Height ),
                    GraphicsUnit.Pixel );
                graphics.Flush ( );
            }

            return bpm;
        }

        public static Bitmap grayscale_copy ( this Image sourceImage )
        {
            var bpm = grab_arb_copy ( sourceImage );
            var bmpData = bpm.LockBits (
                new Rectangle ( 0, 0, sourceImage.Width, sourceImage.Height ),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb );

            var ptr = bmpData.Scan0;

            var byteBuffer = new byte[bmpData.Stride * bpm.Height];

            Marshal.Copy ( ptr, byteBuffer, 0, byteBuffer.Length );

            for ( var k = 0; k < byteBuffer.Length; k += 4 ) {
                var rgb = byteBuffer[k] * 0.11f;
                rgb += byteBuffer[k + 1] * 0.59f;
                rgb += byteBuffer[k + 2] * 0.3f;

                byteBuffer[k] = ( byte ) rgb;
                byteBuffer[k + 1] = byteBuffer[k];
                byteBuffer[k + 2] = byteBuffer[k];

                byteBuffer[k + 3] = 255;
            }

            Marshal.Copy ( byteBuffer, 0, ptr, byteBuffer.Length );

            bpm.UnlockBits ( bmpData );

            return bpm;
        }

    }

}