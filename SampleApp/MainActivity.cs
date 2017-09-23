using Android.Support.V7.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Theartofdev.Edmodo.Cropper;
using Android.App;
using Android.Support.V4.Widget;
using Android.Content.PM;
using Android;
using Java.Interop;
using Android.Net;

namespace App4
{
    [Activity(Label = "ImageCropperQs", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {

        DrawerLayout mDrawerLayout;
        ActionBarDrawerToggle mDrawerToggle;
        MainFragment mCurrentFragment;
        Uri mCropImageUri;
        CropImageViewOptions mCropImageViewOptions = new CropImageViewOptions();

        public void SetCurrentFragment(MainFragment fragment)
        {
            mCurrentFragment = fragment;
        }

        public void SetCurrentOptions(CropImageViewOptions options)
        {
            mCropImageViewOptions = options;
            UpdateDrawerTogglesByOptions(options);
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            mDrawerLayout = (DrawerLayout)FindViewById(Resource.Id.drawer_layout);

            mDrawerToggle = new ActionBarDrawerToggle(this, mDrawerLayout, Resource.String.main_drawer_open, Resource.String.main_drawer_close);
            mDrawerToggle.DrawerIndicatorEnabled = (true);
            mDrawerLayout.SetDrawerListener(mDrawerToggle);

            if (savedInstanceState == null)
            {
                SetMainFragmentByPreset(CropDemoPreset.RECT);
            }
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            mDrawerToggle.SyncState();
            mCurrentFragment.UpdateCurrentCropViewOptions();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (mDrawerToggle.OnOptionsItemSelected(item))
            {
                return true;
            }
            if (mCurrentFragment != null && mCurrentFragment.OnOptionsItemSelected(item))
            {
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == CropImage.PickImageChooserRequestCode && resultCode == Result.Ok)
            {
                Uri imageUri = CropImage.GetPickImageResultUri(this, data);

                // For API >= 23 we need to check specifically that we have permissions to read external storage,
                // but we don't know if we need to for the URI so the simplest is to try open the stream and see if we get error.
                bool requirePermissions = false;
                if (CropImage.IsReadExternalStoragePermissionsRequired(this, imageUri))
                {
                    requirePermissions = true;
                    mCropImageUri = imageUri;
                    RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage }, CropImage.PickImagePermissionsRequestCode);
                }
                else
                {
                    mCurrentFragment.SetImageUri(imageUri);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == CropImage.CameraCapturePermissionsRequestCode)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    CropImage.StartPickImageActivity(this);
                }
                else
                {
                    Toast.MakeText(this, "Cancelling, required permissions are not granted", ToastLength.Short).Show();
                }
            }
            if (requestCode == CropImage.PickImagePermissionsRequestCode)
            {
                if (mCropImageUri != null && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    mCurrentFragment.SetImageUri(mCropImageUri);
                }
                else
                {
                    Toast.MakeText(this, "Cancelling, required permissions are not granted", ToastLength.Short).Show();
                }
            }
        }

        [Export("onDrawerOptionClicked")]
        public void OnDrawerOptionClicked(View view)
        {
            switch (view.Id)
            {
                case Resource.Id.drawer_option_load:
                    if (CropImage.IsExplicitCameraPermissionRequired(this))
                    {
                        RequestPermissions(new string[] { Manifest.Permission.Camera }, CropImage.CameraCapturePermissionsRequestCode);
                    }
                    else
                    {
                        CropImage.StartPickImageActivity(this);
                    }
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_oval:
                    SetMainFragmentByPreset(CropDemoPreset.CIRCULAR);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_rect:
                    SetMainFragmentByPreset(CropDemoPreset.RECT);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_customized_overlay:
                    SetMainFragmentByPreset(CropDemoPreset.CUSTOMIZED_OVERLAY);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_min_max_override:
                    SetMainFragmentByPreset(CropDemoPreset.MIN_MAX_OVERRIDE);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_scale_center:
                    SetMainFragmentByPreset(CropDemoPreset.SCALE_CENTER_INSIDE);
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_toggle_scale:
                    mCropImageViewOptions.scaleType = mCropImageViewOptions.scaleType == CropImageView.ScaleType.FitCenter
                        ? CropImageView.ScaleType.CenterInside : mCropImageViewOptions.scaleType == CropImageView.ScaleType.CenterInside
                        ? CropImageView.ScaleType.Center : mCropImageViewOptions.scaleType == CropImageView.ScaleType.Center
                        ? CropImageView.ScaleType.CenterCrop : CropImageView.ScaleType.FitCenter;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_shape:
                    mCropImageViewOptions.cropShape = mCropImageViewOptions.cropShape == CropImageView.CropShape.Rectangle
                        ? CropImageView.CropShape.Oval : CropImageView.CropShape.Rectangle;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_guidelines:
                    mCropImageViewOptions.guidelines = mCropImageViewOptions.guidelines == CropImageView.Guidelines.Off
                        ? CropImageView.Guidelines.On : mCropImageViewOptions.guidelines == CropImageView.Guidelines.On
                        ? CropImageView.Guidelines.OnTouch : CropImageView.Guidelines.Off;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_aspect_ratio:
                    if (!mCropImageViewOptions.fixAspectRatio)
                    {
                        mCropImageViewOptions.fixAspectRatio = true;
                        mCropImageViewOptions.aspectRatio = new System.Tuple<int, int>(1, 1);
                    }
                    else
                    {
                        if (mCropImageViewOptions.aspectRatio.Item1 == 1 && mCropImageViewOptions.aspectRatio.Item2 == 1)
                        {
                            mCropImageViewOptions.aspectRatio = new System.Tuple<int, int>(4, 3);
                        }
                        else if (mCropImageViewOptions.aspectRatio.Item1 == 4 && mCropImageViewOptions.aspectRatio.Item2 == 3)
                        {
                            mCropImageViewOptions.aspectRatio = new System.Tuple<int, int>(16, 9);
                        }
                        else if (mCropImageViewOptions.aspectRatio.Item1 == 16 && mCropImageViewOptions.aspectRatio.Item2 == 9)
                        {
                            mCropImageViewOptions.aspectRatio = new System.Tuple<int, int>(9, 16);
                        }
                        else
                        {
                            mCropImageViewOptions.fixAspectRatio = false;
                        }
                    }
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_auto_zoom:
                    mCropImageViewOptions.autoZoomEnabled = !mCropImageViewOptions.autoZoomEnabled;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_max_zoom:
                    mCropImageViewOptions.maxZoomLevel = mCropImageViewOptions.maxZoomLevel == 4 ? 8
                            : mCropImageViewOptions.maxZoomLevel == 8 ? 2 : 4;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_set_initial_crop_rect:
                    mCurrentFragment.SetInitialCropRect();
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_reset_crop_rect:
                    mCurrentFragment.ResetCropRect();
                    mDrawerLayout.CloseDrawers();
                    break;
                case Resource.Id.drawer_option_toggle_multitouch:
                    mCropImageViewOptions.multitouch = !mCropImageViewOptions.multitouch;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_show_overlay:
                    mCropImageViewOptions.showCropOverlay = !mCropImageViewOptions.showCropOverlay;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                case Resource.Id.drawer_option_toggle_show_progress_bar:
                    mCropImageViewOptions.showProgressBar = !mCropImageViewOptions.showProgressBar;
                    mCurrentFragment.SetCropImageViewOptions(mCropImageViewOptions);
                    UpdateDrawerTogglesByOptions(mCropImageViewOptions);
                    break;
                default:
                    Toast.MakeText(this, "Unknown drawer option clicked", ToastLength.Long).Show();
                    break;
            }
        }

        void SetMainFragmentByPreset(CropDemoPreset demoPreset)
        {
            SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.container, MainFragment.NewInstance(demoPreset))
                    .Commit();
        }

        void UpdateDrawerTogglesByOptions(CropImageViewOptions options)
        {
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_scale)).Text =Resources.GetString(Resource.String.drawer_option_toggle_scale, options.scaleType.ToString());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_shape)).Text =Resources.GetString(Resource.String.drawer_option_toggle_shape, options.cropShape.ToString());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_guidelines)).Text =Resources.GetString(Resource.String.drawer_option_toggle_guidelines, options.guidelines.ToString());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_multitouch)).Text =Resources.GetString(Resource.String.drawer_option_toggle_multitouch, options.multitouch.ToString());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_show_overlay)).Text =Resources.GetString(Resource.String.drawer_option_toggle_show_overlay, options.showCropOverlay.ToString());
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_show_progress_bar)).Text =Resources.GetString(Resource.String.drawer_option_toggle_show_progress_bar, options.showProgressBar.ToString());

            string aspectRatio = "FREE";
            if (options.fixAspectRatio)
            {
                aspectRatio = options.aspectRatio.Item1 + ":" + options.aspectRatio.Item2;
            }
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_aspect_ratio)).Text =Resources.GetString(Resource.String.drawer_option_toggle_aspect_ratio, aspectRatio);
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_auto_zoom)).Text =Resources.GetString(Resource.String.drawer_option_toggle_auto_zoom, options.autoZoomEnabled ? "Enabled" : "Disabled");
            ((TextView)FindViewById(Resource.Id.drawer_option_toggle_max_zoom)).Text =Resources.GetString(Resource.String.drawer_option_toggle_max_zoom, options.maxZoomLevel);
        }
    }
}