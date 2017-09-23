using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Net;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Java.Lang;

namespace App4
{
    public class MainFragment : Android.Support.V4.App.Fragment, CropImageView.IOnSetImageUriCompleteListener, CropImageView.IOnCropImageCompleteListener
    {
        CropDemoPreset mDemoPreset;
        CropImageView mCropImageView;

        public static MainFragment NewInstance(CropDemoPreset demoPreset)
        {
            MainFragment fragment = new MainFragment();
            Bundle args = new Bundle();
            args.PutInt("DEMO_PRESET", (int)demoPreset);
            fragment.Arguments = args;
            return fragment;
        }


        public void SetImageUri(Uri imageUri)
        {
            mCropImageView.SetImageUriAsync(imageUri);
        }

        public void SetCropImageViewOptions(CropImageViewOptions options)
        {
            mCropImageView.SetScaleType(options.scaleType);
            mCropImageView.SetCropShape(options.cropShape);
            mCropImageView.SetGuidelines(options.guidelines);
            mCropImageView.SetAspectRatio(options.aspectRatio.Item1, options.aspectRatio.Item2);
            mCropImageView.SetFixedAspectRatio(options.fixAspectRatio);
            mCropImageView.SetMultiTouchEnabled(options.multitouch);
            mCropImageView.ShowCropOverlay = (options.showCropOverlay);
            mCropImageView.ShowProgressBar = (options.showProgressBar);
            mCropImageView.AutoZoomEnabled = (options.autoZoomEnabled);
            mCropImageView.MaxZoom = (options.maxZoomLevel);
            mCropImageView.FlippedHorizontally = (options.flipHorizontally);
            mCropImageView.FlippedVertically = (options.flipVertically);
        }

        public void SetInitialCropRect()
        {
            mCropImageView.CropRect = (new Rect(100, 300, 500, 1200));
        }

        public void ResetCropRect()
        {
            mCropImageView.ResetCropRect();
        }

        public void UpdateCurrentCropViewOptions()
        {
            CropImageViewOptions options = new CropImageViewOptions();
            options.scaleType = mCropImageView.GetScaleType();
            options.cropShape = mCropImageView.GetCropShape();
            options.guidelines = mCropImageView.GetGuidelines();
            options.aspectRatio = new System.Tuple<int, int>((int)mCropImageView.AspectRatio.First, (int)mCropImageView.AspectRatio.Second);
            options.fixAspectRatio = mCropImageView.IsFixAspectRatio;
            options.showCropOverlay = mCropImageView.ShowCropOverlay;
            options.showProgressBar = mCropImageView.ShowProgressBar;
            options.autoZoomEnabled = mCropImageView.AutoZoomEnabled;
            options.maxZoomLevel = mCropImageView.MaxZoom;
            options.flipHorizontally = mCropImageView.FlippedHorizontally;
            options.flipVertically = mCropImageView.FlippedVertically;
            ((MainActivity)Activity).SetCurrentOptions(options);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView;
            switch (mDemoPreset)
            {
                case CropDemoPreset.RECT:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_rect, container, false);
                    break;
                case CropDemoPreset.CIRCULAR:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_oval, container, false);
                    break;
                case CropDemoPreset.CUSTOMIZED_OVERLAY:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_customized, container, false);
                    break;
                case CropDemoPreset.MIN_MAX_OVERRIDE:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_min_max, container, false);
                    break;
                case CropDemoPreset.SCALE_CENTER_INSIDE:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_scale_center, container, false);
                    break;
                case CropDemoPreset.CUSTOM:
                    rootView = inflater.Inflate(Resource.Layout.fragment_main_rect, container, false);
                    break;
                default:
                    throw new IllegalStateException("Unknown preset: " + mDemoPreset);
            }
            return rootView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            mCropImageView = (CropImageView)view.FindViewById(Resource.Id.cropImageView);
            mCropImageView.SetOnSetImageUriCompleteListener(this);
            mCropImageView.SetOnCropImageCompleteListener(this);

            UpdateCurrentCropViewOptions();

            if (savedInstanceState == null)
            {
                if (mDemoPreset == CropDemoPreset.SCALE_CENTER_INSIDE)
                {
                    mCropImageView.ImageResource = (Resource.Drawable.cat_small);
                }
                else
                {
                    mCropImageView.ImageResource = (Resource.Drawable.cat);
                }
            }
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.main_action_crop)
            {
                mCropImageView.GetCroppedImageAsync();
                return true;
            }
            else if (item.ItemId == Resource.Id.main_action_rotate)
            {
                mCropImageView.RotateImage(90);
                return true;
            }
            else if (item.ItemId == Resource.Id.main_action_flip_horizontally)
            {
                mCropImageView.FlipImageHorizontally();
                return true;
            }
            else if (item.ItemId == Resource.Id.main_action_flip_vertically)
            {
                mCropImageView.FlipImageVertically();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
            mDemoPreset = (CropDemoPreset)(Arguments.GetInt("DEMO_PRESET"));
            ((MainActivity)activity).SetCurrentFragment(this);
        }

        public override void OnDetach()
        {
            base.OnDetach();
            if (mCropImageView != null)
            {
                mCropImageView.SetOnSetImageUriCompleteListener(null);
                mCropImageView.SetOnCropImageCompleteListener(null);
            }
        }

        public void OnSetImageUriComplete(CropImageView view, Uri uri, Exception error)
        {
            if (error == null)
            {
                Toast.MakeText(Activity, "Image load successful", ToastLength.Short).Show();
            }
            else
            {
                Log.Error("AIC", "Failed to load image by URI", error);
                Toast.MakeText(Activity, "Image load failed: " + error.Message, ToastLength.Long).Show();
            }
        }

        public void OnCropImageComplete(CropImageView view, CropImageView.CropResult result)
        {
            HandleCropResult(result);
        }

        public void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == CropImage.CropImageActivityRequestCode)
            {
                CropImage.ActivityResult result = CropImage.GetActivityResult(data);
                HandleCropResult(result);
            }
        }

        void HandleCropResult(CropImageView.CropResult result)
        {
            if (result.Error == null)
            {
                Intent intent = new Intent(Activity, typeof(CropResultActivity));
                intent.PutExtra("SAMPLE_SIZE", result.SampleSize);
                if (result.Uri != null)
                {
                    intent.PutExtra("URI", result.Uri);
                }
                else
                {
                    CropResultActivity.mImage = mCropImageView.GetCropShape() == CropImageView.CropShape.Oval
                        ? CropImage.ToOvalBitmap(result.Bitmap)
                        : result.Bitmap;
                }

                StartActivity(intent);
            }
            else
            {
                Log.Error("AIC", "Failed to crop image", result.Error);
                Toast.MakeText(Activity, "Image crop failed: " + result.Error.Message, ToastLength.Short).Show();
            }
        }
    }
}