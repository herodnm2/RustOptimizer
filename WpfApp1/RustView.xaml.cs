using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public partial class RustView : UserControl
    {
        public RustView()
        {
            InitializeComponent();
            NotifyBox.RenderTransform = new TranslateTransform(0, 20);
        }
        bool _steamConfirmResult = false;
        
        public static string? GetSteamPath()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            return key?.GetValue("SteamPath") as string;
        }
        void CloseSteam()
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.ToLower().Contains("steam"))
                {
                    try { p.Kill(); } catch { }
                }
            }

            System.Threading.Thread.Sleep(2000);
        }
        bool AskCloseSteam()
        {
            ConfirmOverlay.Visibility = Visibility.Visible;
            _confirmResult = false;

            var frame = new System.Windows.Threading.DispatcherFrame();

            EventHandler handler = null;
            handler = (s, e) =>
            {
                frame.Continue = false;
            };

            ConfirmOverlay.Tag = handler;

            System.Windows.Threading.Dispatcher.PushFrame(frame);

            return _confirmResult;
        }
        bool _confirmResult = false;
        private void ConfirmCloseSteam(object sender, RoutedEventArgs e)
        {
            _confirmResult = true;
            ConfirmOverlay.Visibility = Visibility.Collapsed;

            if (ConfirmOverlay.Tag is EventHandler h)
                h(null, EventArgs.Empty);
        }

        private void CancelCloseSteam(object sender, RoutedEventArgs e)
        {
            _confirmResult = false;
            ConfirmOverlay.Visibility = Visibility.Collapsed;

            if (ConfirmOverlay.Tag is EventHandler h)
                h(null, EventArgs.Empty);
        }
       
        public static string? FindRustPath()
        {
            string? steamPath = GetSteamPath();
            if (steamPath == null) return null;

            string libraryFile = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");
            if (!File.Exists(libraryFile)) return null;

            foreach (var line in File.ReadAllLines(libraryFile))
            {
                if (line.Contains("path"))
                {
                    string path = line.Split('"')[3].Replace(@"\\", @"\");
                    string rustPath = Path.Combine(path, @"steamapps\common\Rust");

                    if (Directory.Exists(rustPath))
                        return rustPath;
                }
            }

            return null;
        }

      
        void ApplyConfig(string config)
        {
            var rustPath = FindRustPath();

            if (rustPath == null)
            {
                MessageBox.Show("Rust not found!");
                return;
            }

            string cfgFolder = Path.Combine(rustPath, "cfg");
            Directory.CreateDirectory(cfgFolder);

            string file = Path.Combine(cfgFolder, "client.cfg");

            File.WriteAllText(file, config);
        }

       
        void SetLaunchOptions(string options)
        {
            var steamPath = GetSteamPath();
            if (steamPath == null) return;

            string userdata = Path.Combine(steamPath, "userdata");

            foreach (var userDir in Directory.GetDirectories(userdata))
            {
                string configPath = Path.Combine(userDir, @"config\localconfig.vdf");

                if (!File.Exists(configPath))
                    continue;

                string text = File.ReadAllText(configPath);

                if (text.Contains("\"252490\""))
                {
                    if (text.Contains("\"LaunchOptions\""))
                    {
                        text = Regex.Replace(
                            text,
                            "\"LaunchOptions\"\\s*\".*?\"",
                            $"\"LaunchOptions\" \"{options}\"");
                    }
                    else
                    {
                        text = text.Replace(
                            "\"252490\"",
                            $"\"252490\"\n\t\t{{\n\t\t\t\"LaunchOptions\" \"{options}\"");
                    }

                    File.WriteAllText(configPath, text);
                }
            }
        }

       
        private void PresetChanged(object sender, RoutedEventArgs e)
        {
            if (MinRadio.IsChecked == true)
            {
                PreviewImage.Source = new BitmapImage(new Uri("min.jpg", UriKind.RelativeOrAbsolute));
                PresetLabel.Text = "MIN";
            }
            else if (MidRadio.IsChecked == true)
            {
                PreviewImage.Source = new BitmapImage(new Uri("mid.jpg", UriKind.RelativeOrAbsolute));
                PresetLabel.Text = "MEDIUM";
            }
            else if (MaxRadio.IsChecked == true)
            {
                PreviewImage.Source = new BitmapImage(new Uri("max.jpg", UriKind.RelativeOrAbsolute));
                PresetLabel.Text = "HIGH";
            }
        }

        
        private void ApplyPreset_Click(object sender, RoutedEventArgs e)
        {
            if (!AskCloseSteam())
            {
                ShowNotify("Operation cancelled");
                return;
            }
            CloseSteam();
            if (MinRadio.IsChecked == true)
            {
                ApplyConfig(
@"accessibility.allynametagcolour ""0""
accessibility.buildingblockedzonecolour ""0""
accessibility.clannametagcolour ""0""
accessibility.disablemovementininventory ""False""
accessibility.enemynametagcolour ""0""
accessibility.healthbarcolour ""0""
accessibility.holosightcolour ""0""
accessibility.hungerbarcolour ""0""
accessibility.hydrationbarcolour ""0""
accessibility.ioarrowinputcolor ""0""
accessibility.ioarrowoutputcolor ""0""
accessibility.laserdetectorcolour ""0""
accessibility.mushroomcolour ""0""
accessibility.senduinavigationevents ""False""
accessibility.teamnametagcolour ""0""
accessibility.treemarkercolor ""2""
audio.advancedocclusion ""False""
audio.eventaudio ""0""
audio.game ""1""
audio.instruments ""0""
audio.master ""0.46""
audio.musicvolume ""0""
audio.musicvolumemenu ""0""
audio.speakers ""2""
audio.ui ""0.77""
audio.voiceprops ""0""
audio.voices ""1""
client.allowcameratiltondpv ""False""
client.allowdiscordprovisionalaccount ""True""
client.allowteaminvitesremoteplayers ""True""
client.autoconnect """"
client.autosavepaintinginterval ""30""
client.autosavepaintings ""True""
client.bag_unclaim_duration ""0.1""
client.bagassignmode ""0""
client.building_guide_mode ""1""
client.buildingskin ""0""
client.buildingskinmetal ""10221""
client.buildingskinstone ""0""
client.buildingskintoptier ""0""
client.buildingskinwood ""0""
client.cached_browser_print_tag_errors ""False""
client.cambone """"
client.camdist ""2""
client.camfov ""70""
client.camoffset ""(0.00, 1.00, 0.00)""
client.camoffset_relative ""False""
client.clampscreenshake ""True""
client.crosshair ""True""
client.drawrangevolumes ""True""
client.enablefriendslogging ""False""
client.errortoasts_in_chat ""False""
client.hammerhealthinfodisplaying ""True""
client.hascompletedtutorial ""True""
client.hasdeclinedtutorial ""True""
client.headbob ""False""
client.hidedmsinstreamermode ""False""
client.hitcross ""True""
client.hurtpunch ""False""
client.io_arrow_important_only ""True""
client.io_arrow_mode ""1""
client.lookatradius ""-1""
client.map_marker_autoname ""True""
client.map_marker_color ""-1""
client.orbitcamdist ""2""
client.orbitcamlookspeed ""50""
client.pushtotalk ""True""
client.rockskin ""10315""
client.selectedshippingcontainerblockcolour ""1""
client.server_restart_warnings_in_chat ""False""
client.showcaminfo ""False""
client.showgrowableui ""True""
client.showmissionprovidersonmap ""True""
client.showsleepingbagsonmap ""True""
client.showtogglegrowableui ""True""
client.showvendingmachinesonmap ""True""
client.sortskinsrecentlyused ""True""
client.torchskin ""0""
client.underwearskin ""241501709""
console.erroroverlay ""False""
culling.entitymaxdist ""5000""
culling.entityminculldist ""15""
culling.entityminshadowculldist ""5""
culling.entityupdaterate ""5""
culling.env ""True""
culling.envmindist ""10""
culling.safemode ""False""
culling.toggle ""True""
ddraw.hideddrawduringdemo ""False""
debug.debugcamera_autoload ""False""
debug.debugcamera_autosave ""False""
debug.debugcamera_offset ""(0.00, 0.00, 0.00)""
debug.debugcamera_preserve ""False""
debug.invokeperformancetracking ""False""
debug.showviewmodelaimhelper ""False""
debug.showworldinfoinperformancereadout ""False""
debug.viewmodelaimhelpwidth ""4""
decor.quality ""0""
demo.autodebugcam ""False""
demo.compressshotkeyframes ""False""
demo.showcommunityui ""False""
demo.showlocalplayernametag ""False""
demo.ui ""True""
effects.bloom ""False""
effects.creationeffects ""False""
effects.hurtoverlay ""False""
effects.hurtoverleyapplylighting ""False""
effects.lensdirt ""False""
effects.maxgibdist ""150""
effects.maxgiblife ""0""
effects.maxgibs ""0""
effects.mingiblife ""0""
effects.motionblur ""False""
effects.otherplayerslightflares ""True""
effects.shafts ""False""
effects.showoutlines ""True""
effects.vignet ""False""
fps.limit ""240""
fps.limitinbackground ""True""
fps.limitinmenu ""True""
gc.buffer ""2020""
global.aquaticvehicledismounttime ""0""
global.blockemoji ""False""
global.blockemojianimations ""False""
global.blockserveremoji ""False""
global.censornudity ""2""
global.censorrecordings ""False""
global.censorsigns ""False""
global.consolescale ""16""
global.debuglanguage ""0""
global.flyingvehicledismounttime ""0""
global.god ""False""
global.godforceoffoverlay ""False""
global.groundvehicledismounttime ""0""
global.hideinteracttextwhileads ""False""
global.hideteamleadermapmarkers ""False""
global.horsedismounttime ""0""
global.language ""ru""
global.limitflashing ""False""
global.perf ""4""
global.processmidiinput ""False""
global.richpresence ""True""
global.showblood ""False""
global.showdeathmarkeroncompass ""True""
global.showemojierrors ""False""
global.showitemcountsonpickup ""True""
global.signundobuffer ""10""
global.streamermode ""False""
global.usesingleitempickupnotice ""True""
graphics.aggressiveshadowlod ""True""
graphics.aggressiveshadowlodwearable ""True""
graphics.branding ""False""
graphics.chat ""True""
graphics.collapserenderers ""True""
graphics.compass ""1""
graphics.dlaa ""False""
graphics.dlss ""-1""
graphics.dof ""False""
graphics.dof_aper ""12""
graphics.dof_barrel ""0""
graphics.dof_blur ""1""
graphics.dof_debug ""False""
graphics.dof_focus_dist ""10""
graphics.dof_focus_time ""0.2""
graphics.dof_kernel_count ""0""
graphics.dof_mode ""0""
graphics.dof_squeeze ""0""
graphics.drawdistance ""1000""
graphics.fov ""90""
graphics.hlod ""True""
graphics.impostorshadows ""False""
graphics.maxqueuedframes ""2""
graphics.parallax ""0""
graphics.reflexintervalus ""0""
graphics.reflexmode ""0""
graphics.renderscale ""1""
graphics.resolution ""0""
graphics.screenmode ""1""
graphics.shadowfilteringquality ""1""
graphics.uiscale ""0.95""
graphics.viewmodeldepth ""True""
graphics.vm_fov_scale ""False""
graphics.vm_horizontal_flip ""False""
graphics.vsync ""0""
graphicssettings.billboardsfacecameraposition ""False""
graphicssettings.enablelodcrossfade ""True""
graphicssettings.particleraycastbudget ""4""
graphicssettings.pixellightcount ""0""
graphicssettings.shadowcascades ""2""
graphicssettings.shadowdistancepercent ""100""
graphicssettings.shadowresolution ""0""
graphicssettings.softparticles ""True""
grass.distance ""100""
grass.refresh_budget ""0.3""
indirect_instancing.debug ""False""
indirect_instancing.debug_motion ""False""
indirect_instancing.debug_raycast ""True""
indirect_instancing.enabled ""False""
input.ads_sensitivity ""0.8""
input.alwayssprint ""False""
input.autocrouch ""False""
input.flipy ""False""
input.holdtime ""0.2""
input.map_mode ""0""
input.radial_menu_mode ""0""
input.sensitivity ""0.96""
input.toggleads ""False""
input.toggleduck ""False""
input.vehicle_flipy ""False""
input.vehicle_sensitivity ""1""
instruments.processsustainpedal ""True""
inventory.quickcraft_button_delay ""0""
inventory.quickcraft_rebuild_delay ""0""
legs.enablelegs ""False""
lod.grid_refresh_budget ""0.1""
netgraph.enabled ""False""
netgraph.updatespeed ""5""
paint.favcolours ""#95BF4B, #CD412B, #1F6BA0""
paint.maxbrushsize ""100""
particle.quality ""0""
party.party_invites_enabled ""True""
player.cold_breath ""False""
player.footik ""False""
player.footikdistance ""30""
player.footikrate ""0.1""
player.noclipspeed ""10""
player.noclipspeedfast ""50""
player.noclipspeedslow ""2""
player.recoilcomp ""True""
playercull.enabled ""True""
playercull.maxplayerdist ""5000""
playercull.maxsleeperdist ""30""
playercull.minculldist ""20""
playercull.updaterate ""5""
playercull.visquality ""2""
reflection.planarcount ""2""
reflection.planarresolution ""1024""
render.instanced_rendering ""0""
render.instancing_render_distance ""1000""
render.show_building_blocked ""True""
sss.enabled ""True""
sss.halfres ""True""
sss.quality ""0""
sss.scale ""1""
steam.use_steam_nicknames ""True""
store.preloadweeklyskins ""True""
system.auto_cpu_affinity ""True""
ui.autoswitchchannel ""True""
ui.monumentnotificationtoasts ""False""
ui.scrollsensitivity ""1""
ui.showbeltbarbinds ""False""
ui.showinventoryplayer ""True""
ui.showusebind ""False""
voice.loopback ""False""
crosshair.color ""3""
crosshair.dotsize ""1.7""
crosshair.dynamic_spacing ""True""
crosshair.dynamic_visibility ""True""
crosshair.length ""7""
crosshair.outline ""False""
crosshair.outlinecolor ""0""
crosshair.spacing ""1""
crosshair.style ""1""
crosshair.width ""2""
espplayerinfo.blueteamid ""3000""
espplayerinfo.grayteamid ""7000""
espplayerinfo.greenteamid ""1000""
espplayerinfo.lavenderteamid ""9000""
espplayerinfo.mintteamid ""10000""
espplayerinfo.orangeteamid ""6000""
espplayerinfo.pinkteamid ""8000""
espplayerinfo.purpleteamid ""5000""
espplayerinfo.redteamid ""2000""
espplayerinfo.yellowteamid ""4000""
gametip.server_event_tips ""True""
gametip.showgametips ""False""
gesturecollection.showadmincinematicgesturesinbindings ""False""
gesturecollection.slot0ring0bind ""clap""
gesturecollection.slot0ring1bind """"
gesturecollection.slot10ring0bind ""raiseroof""
gesturecollection.slot10ring1bind """"
gesturecollection.slot11ring0bind ""cabbagepatch""
gesturecollection.slot11ring1bind """"
gesturecollection.slot12ring0bind ""twist""
gesturecollection.slot12ring1bind """"
gesturecollection.slot1ring0bind ""surrender""
gesturecollection.slot1ring1bind """"
gesturecollection.slot2ring0bind ""hurry""
gesturecollection.slot2ring1bind """"
gesturecollection.slot3ring0bind ""ok""
gesturecollection.slot3ring1bind """"
gesturecollection.slot4ring0bind ""point""
gesturecollection.slot4ring1bind """"
gesturecollection.slot5ring0bind ""shrug""
gesturecollection.slot5ring1bind """"
gesturecollection.slot6ring0bind ""thumbsdown""
gesturecollection.slot6ring1bind """"
gesturecollection.slot7ring0bind ""thumbsup""
gesturecollection.slot7ring1bind """"
gesturecollection.slot8ring0bind ""victory""
gesturecollection.slot8ring1bind """"
gesturecollection.slot9ring0bind ""beatchest""
gesturecollection.slot9ring1bind """"
lookattooltip.crosshairmode ""0""
megaphone.ignorepushtotalk ""True""
metaldetectorsource.draweditorgizmos ""False""
midiconvar.bufferprotection ""False""
midiconvar.debugmode ""False""
midiconvar.enabled ""False""
nametags.enabled ""True""
projectile.preventcameraclip ""True""
recordertool.debugrecording ""False""
rgbeffects.brightness ""0""
rgbeffects.colorcorrection_razer ""(3.00, 3.00, 3.00)""
rgbeffects.colorcorrection_steelseries ""(1.50, 1.50, 1.50)""
rgbeffects.enabled ""False""
keyboardmidi.midikeymap ""qwerty-uk.json""
ui_dropscontroller.show_placeholder_drop_data ""False""
screenshot.hiresscreenshotcustomwidth ""0""
shoutcaststreamer.allowinternetstreams ""False""
shoutcaststreamer.maxaudiostreams ""3""
socket_free_snappable.snappingmode ""2""
strobelight.forceoff ""True""
toolgun.classiceffects ""False""
graphics.shadowlights ""1""
graphicssettings.shadowqualitypreset ""0""
graphicssettings.globaltexturemipmaplimit ""3""
graphics.af ""1""
graphics.lodbias ""0.5""
graphics.shaderlod ""1""
graphicssettings.anisotropicfiltering ""0""
mesh.quality ""0""
terrain.quality ""100""
graphics.contactshadows ""False""
effects.ao ""False""
tree.meshes ""10""
tree.quality ""30""
water.quality ""0""
water.reflections ""0""
grass.displacement ""True""
grass.quality ""0""
graphics.grassshadows ""False""
graphics.volumetric_clouds ""0""
effects.sharpen ""True""
effects.antialiasing ""0""
reflection.planarreflections ""True""");

                SetLaunchOptions("-player.eye_blinking False -player.eye_movement False -client.headlerp 10 -headlerp_inertia 0 -global.enable_marker_teleport True -graphics.shadowmode 1");
                ShowNotify("Minimum settings applied!");
            }
            else if (MidRadio.IsChecked == true)
            {
                ApplyConfig(
@"accessibility.allynametagcolour ""0""
accessibility.buildingblockedzonecolour ""0""
accessibility.clannametagcolour ""0""
accessibility.disablemovementininventory ""False""
accessibility.enemynametagcolour ""0""
accessibility.healthbarcolour ""0""
accessibility.holosightcolour ""0""
accessibility.hungerbarcolour ""0""
accessibility.hydrationbarcolour ""0""
accessibility.ioarrowinputcolor ""0""
accessibility.ioarrowoutputcolor ""0""
accessibility.laserdetectorcolour ""0""
accessibility.mushroomcolour ""0""
accessibility.senduinavigationevents ""False""
accessibility.teamnametagcolour ""0""
accessibility.treemarkercolor ""2""
audio.advancedocclusion ""False""
audio.eventaudio ""0""
audio.game ""1""
audio.instruments ""0""
audio.master ""0.46""
audio.musicvolume ""0""
audio.musicvolumemenu ""0""
audio.speakers ""2""
audio.ui ""0.77""
audio.voiceprops ""0""
audio.voices ""1""
client.allowcameratiltondpv ""False""
client.allowdiscordprovisionalaccount ""True""
client.allowteaminvitesremoteplayers ""True""
client.autoconnect """"
client.autosavepaintinginterval ""30""
client.autosavepaintings ""True""
client.bag_unclaim_duration ""0.1""
client.bagassignmode ""0""
client.building_guide_mode ""1""
client.buildingskin ""0""
client.buildingskinmetal ""10221""
client.buildingskinstone ""0""
client.buildingskintoptier ""0""
client.buildingskinwood ""0""
client.cached_browser_print_tag_errors ""False""
client.cambone """"
client.camdist ""2""
client.camfov ""70""
client.camoffset ""(0.00, 1.00, 0.00)""
client.camoffset_relative ""False""
client.clampscreenshake ""True""
client.crosshair ""True""
client.drawrangevolumes ""True""
client.enablefriendslogging ""False""
client.errortoasts_in_chat ""False""
client.hammerhealthinfodisplaying ""True""
client.hascompletedtutorial ""True""
client.hasdeclinedtutorial ""True""
client.headbob ""False""
client.hidedmsinstreamermode ""False""
client.hitcross ""True""
client.hurtpunch ""False""
client.io_arrow_important_only ""True""
client.io_arrow_mode ""1""
client.lookatradius ""-1""
client.map_marker_autoname ""True""
client.map_marker_color ""-1""
client.orbitcamdist ""2""
client.orbitcamlookspeed ""50""
client.pushtotalk ""True""
client.rockskin ""10315""
client.selectedshippingcontainerblockcolour ""1""
client.server_restart_warnings_in_chat ""False""
client.showcaminfo ""False""
client.showgrowableui ""True""
client.showmissionprovidersonmap ""True""
client.showsleepingbagsonmap ""True""
client.showtogglegrowableui ""True""
client.showvendingmachinesonmap ""True""
client.sortskinsrecentlyused ""True""
client.torchskin ""0""
client.underwearskin ""241501709""
console.erroroverlay ""False""
culling.entitymaxdist ""5000""
culling.entityminculldist ""15""
culling.entityminshadowculldist ""5""
culling.entityupdaterate ""5""
culling.env ""True""
culling.envmindist ""10""
culling.safemode ""False""
culling.toggle ""True""
ddraw.hideddrawduringdemo ""False""
debug.debugcamera_autoload ""False""
debug.debugcamera_autosave ""False""
debug.debugcamera_offset ""(0.00, 0.00, 0.00)""
debug.debugcamera_preserve ""False""
debug.invokeperformancetracking ""False""
debug.showviewmodelaimhelper ""False""
debug.showworldinfoinperformancereadout ""False""
debug.viewmodelaimhelpwidth ""4""
decor.quality ""0""
demo.autodebugcam ""False""
demo.compressshotkeyframes ""False""
demo.showcommunityui ""False""
demo.showlocalplayernametag ""False""
demo.ui ""True""
effects.bloom ""False""
effects.creationeffects ""False""
effects.hurtoverlay ""False""
effects.hurtoverleyapplylighting ""False""
effects.lensdirt ""False""
effects.maxgibdist ""150""
effects.maxgiblife ""0""
effects.maxgibs ""0""
effects.mingiblife ""0""
effects.motionblur ""False""
effects.otherplayerslightflares ""True""
effects.shafts ""False""
effects.showoutlines ""True""
effects.vignet ""False""
fps.limit ""240""
fps.limitinbackground ""True""
fps.limitinmenu ""True""
gc.buffer ""2020""
global.aquaticvehicledismounttime ""0""
global.blockemoji ""False""
global.blockemojianimations ""False""
global.blockserveremoji ""False""
global.censornudity ""2""
global.censorrecordings ""False""
global.censorsigns ""False""
global.consolescale ""16""
global.debuglanguage ""0""
global.flyingvehicledismounttime ""0""
global.god ""False""
global.godforceoffoverlay ""False""
global.groundvehicledismounttime ""0""
global.hideinteracttextwhileads ""False""
global.hideteamleadermapmarkers ""False""
global.horsedismounttime ""0""
global.language ""ru""
global.limitflashing ""False""
global.perf ""4""
global.processmidiinput ""False""
global.richpresence ""True""
global.showblood ""False""
global.showdeathmarkeroncompass ""True""
global.showemojierrors ""False""
global.showitemcountsonpickup ""True""
global.signundobuffer ""10""
global.streamermode ""False""
global.usesingleitempickupnotice ""True""
graphics.aggressiveshadowlod ""True""
graphics.aggressiveshadowlodwearable ""True""
graphics.branding ""False""
graphics.chat ""True""
graphics.collapserenderers ""True""
graphics.compass ""1""
graphics.dlaa ""False""
graphics.dlss ""-1""
graphics.dof ""False""
graphics.dof_aper ""12""
graphics.dof_barrel ""0""
graphics.dof_blur ""1""
graphics.dof_debug ""False""
graphics.dof_focus_dist ""10""
graphics.dof_focus_time ""0.2""
graphics.dof_kernel_count ""0""
graphics.dof_mode ""0""
graphics.dof_squeeze ""0""
graphics.drawdistance ""1000""
graphics.fov ""90""
graphics.hlod ""True""
graphics.impostorshadows ""False""
graphics.maxqueuedframes ""2""
graphics.parallax ""0""
graphics.reflexintervalus ""0""
graphics.reflexmode ""0""
graphics.renderscale ""1""
graphics.resolution ""0""
graphics.screenmode ""1""
graphics.shadowfilteringquality ""1""
graphics.uiscale ""0.95""
graphics.viewmodeldepth ""True""
graphics.vm_fov_scale ""False""
graphics.vm_horizontal_flip ""False""
graphics.vsync ""0""
graphicssettings.billboardsfacecameraposition ""False""
graphicssettings.enablelodcrossfade ""True""
graphicssettings.particleraycastbudget ""4""
graphicssettings.pixellightcount ""0""
graphicssettings.shadowcascades ""2""
graphicssettings.shadowdistancepercent ""100""
graphicssettings.shadowresolution ""0""
graphicssettings.softparticles ""True""
grass.distance ""100""
grass.refresh_budget ""0.3""
indirect_instancing.debug ""False""
indirect_instancing.debug_motion ""False""
indirect_instancing.debug_raycast ""True""
indirect_instancing.enabled ""False""
input.ads_sensitivity ""0.8""
input.alwayssprint ""False""
input.autocrouch ""False""
input.flipy ""False""
input.holdtime ""0.2""
input.map_mode ""0""
input.radial_menu_mode ""0""
input.sensitivity ""0.96""
input.toggleads ""False""
input.toggleduck ""False""
input.vehicle_flipy ""False""
input.vehicle_sensitivity ""1""
instruments.processsustainpedal ""True""
inventory.quickcraft_button_delay ""0""
inventory.quickcraft_rebuild_delay ""0""
legs.enablelegs ""False""
lod.grid_refresh_budget ""0.1""
netgraph.enabled ""False""
netgraph.updatespeed ""5""
paint.favcolours ""#95BF4B, #CD412B, #1F6BA0""
paint.maxbrushsize ""100""
particle.quality ""0""
party.party_invites_enabled ""True""
player.cold_breath ""False""
player.footik ""False""
player.footikdistance ""30""
player.footikrate ""0.1""
player.noclipspeed ""10""
player.noclipspeedfast ""50""
player.noclipspeedslow ""2""
player.recoilcomp ""True""
playercull.enabled ""True""
playercull.maxplayerdist ""5000""
playercull.maxsleeperdist ""30""
playercull.minculldist ""20""
playercull.updaterate ""5""
playercull.visquality ""2""
reflection.planarcount ""2""
reflection.planarresolution ""1024""
render.instanced_rendering ""0""
render.instancing_render_distance ""1000""
render.show_building_blocked ""True""
sss.enabled ""True""
sss.halfres ""True""
sss.quality ""0""
sss.scale ""1""
steam.use_steam_nicknames ""True""
store.preloadweeklyskins ""True""
system.auto_cpu_affinity ""True""
ui.autoswitchchannel ""True""
ui.monumentnotificationtoasts ""False""
ui.scrollsensitivity ""1""
ui.showbeltbarbinds ""False""
ui.showinventoryplayer ""True""
ui.showusebind ""False""
voice.loopback ""False""
crosshair.color ""3""
crosshair.dotsize ""1.7""
crosshair.dynamic_spacing ""True""
crosshair.dynamic_visibility ""True""
crosshair.length ""7""
crosshair.outline ""False""
crosshair.outlinecolor ""0""
crosshair.spacing ""1""
crosshair.style ""1""
crosshair.width ""2""
espplayerinfo.blueteamid ""3000""
espplayerinfo.grayteamid ""7000""
espplayerinfo.greenteamid ""1000""
espplayerinfo.lavenderteamid ""9000""
espplayerinfo.mintteamid ""10000""
espplayerinfo.orangeteamid ""6000""
espplayerinfo.pinkteamid ""8000""
espplayerinfo.purpleteamid ""5000""
espplayerinfo.redteamid ""2000""
espplayerinfo.yellowteamid ""4000""
gametip.server_event_tips ""True""
gametip.showgametips ""False""
gesturecollection.showadmincinematicgesturesinbindings ""False""
gesturecollection.slot0ring0bind ""clap""
gesturecollection.slot0ring1bind """"
gesturecollection.slot10ring0bind ""raiseroof""
gesturecollection.slot10ring1bind """"
gesturecollection.slot11ring0bind ""cabbagepatch""
gesturecollection.slot11ring1bind """"
gesturecollection.slot12ring0bind ""twist""
gesturecollection.slot12ring1bind """"
gesturecollection.slot1ring0bind ""surrender""
gesturecollection.slot1ring1bind """"
gesturecollection.slot2ring0bind ""hurry""
gesturecollection.slot2ring1bind """"
gesturecollection.slot3ring0bind ""ok""
gesturecollection.slot3ring1bind """"
gesturecollection.slot4ring0bind ""point""
gesturecollection.slot4ring1bind """"
gesturecollection.slot5ring0bind ""shrug""
gesturecollection.slot5ring1bind """"
gesturecollection.slot6ring0bind ""thumbsdown""
gesturecollection.slot6ring1bind """"
gesturecollection.slot7ring0bind ""thumbsup""
gesturecollection.slot7ring1bind """"
gesturecollection.slot8ring0bind ""victory""
gesturecollection.slot8ring1bind """"
gesturecollection.slot9ring0bind ""beatchest""
gesturecollection.slot9ring1bind """"
lookattooltip.crosshairmode ""0""
megaphone.ignorepushtotalk ""True""
metaldetectorsource.draweditorgizmos ""False""
midiconvar.bufferprotection ""False""
midiconvar.debugmode ""False""
midiconvar.enabled ""False""
nametags.enabled ""True""
projectile.preventcameraclip ""True""
recordertool.debugrecording ""False""
rgbeffects.brightness ""0""
rgbeffects.colorcorrection_razer ""(3.00, 3.00, 3.00)""
rgbeffects.colorcorrection_steelseries ""(1.50, 1.50, 1.50)""
rgbeffects.enabled ""False""
keyboardmidi.midikeymap ""qwerty-uk.json""
ui_dropscontroller.show_placeholder_drop_data ""False""
screenshot.hiresscreenshotcustomwidth ""0""
shoutcaststreamer.allowinternetstreams ""False""
shoutcaststreamer.maxaudiostreams ""3""
socket_free_snappable.snappingmode ""2""
strobelight.forceoff ""True""
toolgun.classiceffects ""False""
graphics.shadowlights ""1""
graphicssettings.shadowqualitypreset ""0""
graphicssettings.globaltexturemipmaplimit ""2""
graphics.af ""2""
graphics.lodbias ""0.6""
graphics.shaderlod ""2""
graphicssettings.anisotropicfiltering ""1""
mesh.quality ""30""
terrain.quality ""100""
graphics.contactshadows ""False""
effects.ao ""False""
tree.meshes ""100""
tree.quality ""150""
water.quality ""0""
water.reflections ""0""
grass.displacement ""True""
grass.quality ""0""
graphics.grassshadows ""False""
graphics.volumetric_clouds ""0""
effects.sharpen ""True""
effects.antialiasing ""0""
reflection.planarreflections ""True""");

                SetLaunchOptions("-player.eye_blinking False -player.eye_movement False -client.headlerp 10 -headlerp_inertia 0 -global.enable_marker_teleport True -graphics.shadowmode 1");
                ShowNotify("Medium settings applied!");
            }
            else if (MaxRadio.IsChecked == true)
            {
                ApplyConfig(
@"accessibility.allynametagcolour ""0""
accessibility.buildingblockedzonecolour ""0""
accessibility.clannametagcolour ""0""
accessibility.disablemovementininventory ""False""
accessibility.enemynametagcolour ""0""
accessibility.healthbarcolour ""0""
accessibility.holosightcolour ""0""
accessibility.hungerbarcolour ""0""
accessibility.hydrationbarcolour ""0""
accessibility.ioarrowinputcolor ""0""
accessibility.ioarrowoutputcolor ""0""
accessibility.laserdetectorcolour ""0""
accessibility.mushroomcolour ""0""
accessibility.senduinavigationevents ""False""
accessibility.teamnametagcolour ""0""
accessibility.treemarkercolor ""2""
audio.advancedocclusion ""False""
audio.eventaudio ""0""
audio.game ""1""
audio.instruments ""0""
audio.master ""0.46""
audio.musicvolume ""0""
audio.musicvolumemenu ""0""
audio.speakers ""2""
audio.ui ""0.77""
audio.voiceprops ""0""
audio.voices ""1""
client.allowcameratiltondpv ""False""
client.allowdiscordprovisionalaccount ""True""
client.allowteaminvitesremoteplayers ""True""
client.autoconnect """"
client.autosavepaintinginterval ""30""
client.autosavepaintings ""True""
client.bag_unclaim_duration ""0.1""
client.bagassignmode ""0""
client.building_guide_mode ""1""
client.buildingskin ""0""
client.buildingskinmetal ""10221""
client.buildingskinstone ""0""
client.buildingskintoptier ""0""
client.buildingskinwood ""0""
client.cached_browser_print_tag_errors ""False""
client.cambone """"
client.camdist ""2""
client.camfov ""70""
client.camoffset ""(0.00, 1.00, 0.00)""
client.camoffset_relative ""False""
client.clampscreenshake ""True""
client.crosshair ""True""
client.drawrangevolumes ""True""
client.enablefriendslogging ""False""
client.errortoasts_in_chat ""False""
client.hammerhealthinfodisplaying ""True""
client.hascompletedtutorial ""True""
client.hasdeclinedtutorial ""True""
client.headbob ""False""
client.hidedmsinstreamermode ""False""
client.hitcross ""True""
client.hurtpunch ""False""
client.io_arrow_important_only ""True""
client.io_arrow_mode ""1""
client.lookatradius ""-1""
client.map_marker_autoname ""True""
client.map_marker_color ""-1""
client.orbitcamdist ""2""
client.orbitcamlookspeed ""50""
client.pushtotalk ""True""
client.rockskin ""10315""
client.selectedshippingcontainerblockcolour ""1""
client.server_restart_warnings_in_chat ""False""
client.showcaminfo ""False""
client.showgrowableui ""True""
client.showmissionprovidersonmap ""True""
client.showsleepingbagsonmap ""True""
client.showtogglegrowableui ""True""
client.showvendingmachinesonmap ""True""
client.sortskinsrecentlyused ""True""
client.torchskin ""0""
client.underwearskin ""241501709""
console.erroroverlay ""False""
culling.entitymaxdist ""5000""
culling.entityminculldist ""15""
culling.entityminshadowculldist ""5""
culling.entityupdaterate ""5""
culling.env ""True""
culling.envmindist ""10""
culling.safemode ""False""
culling.toggle ""True""
ddraw.hideddrawduringdemo ""False""
debug.debugcamera_autoload ""False""
debug.debugcamera_autosave ""False""
debug.debugcamera_offset ""(0.00, 0.00, 0.00)""
debug.debugcamera_preserve ""False""
debug.invokeperformancetracking ""False""
debug.showviewmodelaimhelper ""False""
debug.showworldinfoinperformancereadout ""False""
debug.viewmodelaimhelpwidth ""4""
decor.quality ""0""
demo.autodebugcam ""False""
demo.compressshotkeyframes ""False""
demo.showcommunityui ""False""
demo.showlocalplayernametag ""False""
demo.ui ""True""
effects.bloom ""False""
effects.creationeffects ""False""
effects.hurtoverlay ""False""
effects.hurtoverleyapplylighting ""False""
effects.lensdirt ""False""
effects.maxgibdist ""150""
effects.maxgiblife ""0""
effects.maxgibs ""0""
effects.mingiblife ""0""
effects.motionblur ""False""
effects.otherplayerslightflares ""True""
effects.shafts ""False""
effects.showoutlines ""True""
effects.vignet ""False""
fps.limit ""240""
fps.limitinbackground ""True""
fps.limitinmenu ""True""
gc.buffer ""2020""
global.aquaticvehicledismounttime ""0""
global.blockemoji ""False""
global.blockemojianimations ""False""
global.blockserveremoji ""False""
global.censornudity ""2""
global.censorrecordings ""False""
global.censorsigns ""False""
global.consolescale ""16""
global.debuglanguage ""0""
global.flyingvehicledismounttime ""0""
global.god ""False""
global.godforceoffoverlay ""False""
global.groundvehicledismounttime ""0""
global.hideinteracttextwhileads ""False""
global.hideteamleadermapmarkers ""False""
global.horsedismounttime ""0""
global.language ""ru""
global.limitflashing ""False""
global.perf ""4""
global.processmidiinput ""False""
global.richpresence ""True""
global.showblood ""False""
global.showdeathmarkeroncompass ""True""
global.showemojierrors ""False""
global.showitemcountsonpickup ""True""
global.signundobuffer ""10""
global.streamermode ""False""
global.usesingleitempickupnotice ""True""
graphics.aggressiveshadowlod ""True""
graphics.aggressiveshadowlodwearable ""True""
graphics.branding ""False""
graphics.chat ""True""
graphics.collapserenderers ""True""
graphics.compass ""1""
graphics.dlaa ""False""
graphics.dlss ""-1""
graphics.dof ""False""
graphics.dof_aper ""12""
graphics.dof_barrel ""0""
graphics.dof_blur ""1""
graphics.dof_debug ""False""
graphics.dof_focus_dist ""10""
graphics.dof_focus_time ""0.2""
graphics.dof_kernel_count ""0""
graphics.dof_mode ""0""
graphics.dof_squeeze ""0""
graphics.drawdistance ""1000""
graphics.fov ""90""
graphics.hlod ""True""
graphics.impostorshadows ""False""
graphics.maxqueuedframes ""2""
graphics.parallax ""0""
graphics.reflexintervalus ""0""
graphics.reflexmode ""0""
graphics.renderscale ""1""
graphics.resolution ""0""
graphics.screenmode ""1""
graphics.shadowfilteringquality ""1""
graphics.uiscale ""0.95""
graphics.viewmodeldepth ""True""
graphics.vm_fov_scale ""False""
graphics.vm_horizontal_flip ""False""
graphics.vsync ""0""
graphicssettings.billboardsfacecameraposition ""False""
graphicssettings.enablelodcrossfade ""True""
graphicssettings.particleraycastbudget ""4""
graphicssettings.pixellightcount ""0""
graphicssettings.shadowcascades ""2""
graphicssettings.shadowdistancepercent ""100""
graphicssettings.shadowresolution ""0""
graphicssettings.softparticles ""True""
grass.distance ""100""
grass.refresh_budget ""0.3""
indirect_instancing.debug ""False""
indirect_instancing.debug_motion ""False""
indirect_instancing.debug_raycast ""True""
indirect_instancing.enabled ""False""
input.ads_sensitivity ""0.8""
input.alwayssprint ""False""
input.autocrouch ""False""
input.flipy ""False""
input.holdtime ""0.2""
input.map_mode ""0""
input.radial_menu_mode ""0""
input.sensitivity ""0.96""
input.toggleads ""False""
input.toggleduck ""False""
input.vehicle_flipy ""False""
input.vehicle_sensitivity ""1""
instruments.processsustainpedal ""True""
inventory.quickcraft_button_delay ""0""
inventory.quickcraft_rebuild_delay ""0""
legs.enablelegs ""False""
lod.grid_refresh_budget ""0.1""
netgraph.enabled ""False""
netgraph.updatespeed ""5""
paint.favcolours ""#95BF4B, #CD412B, #1F6BA0""
paint.maxbrushsize ""100""
particle.quality ""0""
party.party_invites_enabled ""True""
player.cold_breath ""False""
player.footik ""False""
player.footikdistance ""30""
player.footikrate ""0.1""
player.noclipspeed ""10""
player.noclipspeedfast ""50""
player.noclipspeedslow ""2""
player.recoilcomp ""True""
playercull.enabled ""True""
playercull.maxplayerdist ""5000""
playercull.maxsleeperdist ""30""
playercull.minculldist ""20""
playercull.updaterate ""5""
playercull.visquality ""2""
reflection.planarcount ""2""
reflection.planarresolution ""1024""
render.instanced_rendering ""0""
render.instancing_render_distance ""1000""
render.show_building_blocked ""True""
sss.enabled ""True""
sss.halfres ""True""
sss.quality ""0""
sss.scale ""1""
steam.use_steam_nicknames ""True""
store.preloadweeklyskins ""True""
system.auto_cpu_affinity ""True""
ui.autoswitchchannel ""True""
ui.monumentnotificationtoasts ""False""
ui.scrollsensitivity ""1""
ui.showbeltbarbinds ""False""
ui.showinventoryplayer ""True""
ui.showusebind ""False""
voice.loopback ""False""
crosshair.color ""3""
crosshair.dotsize ""1.7""
crosshair.dynamic_spacing ""True""
crosshair.dynamic_visibility ""True""
crosshair.length ""7""
crosshair.outline ""False""
crosshair.outlinecolor ""0""
crosshair.spacing ""1""
crosshair.style ""1""
crosshair.width ""2""
espplayerinfo.blueteamid ""3000""
espplayerinfo.grayteamid ""7000""
espplayerinfo.greenteamid ""1000""
espplayerinfo.lavenderteamid ""9000""
espplayerinfo.mintteamid ""10000""
espplayerinfo.orangeteamid ""6000""
espplayerinfo.pinkteamid ""8000""
espplayerinfo.purpleteamid ""5000""
espplayerinfo.redteamid ""2000""
espplayerinfo.yellowteamid ""4000""
gametip.server_event_tips ""True""
gametip.showgametips ""False""
gesturecollection.showadmincinematicgesturesinbindings ""False""
gesturecollection.slot0ring0bind ""clap""
gesturecollection.slot0ring1bind """"
gesturecollection.slot10ring0bind ""raiseroof""
gesturecollection.slot10ring1bind """"
gesturecollection.slot11ring0bind ""cabbagepatch""
gesturecollection.slot11ring1bind """"
gesturecollection.slot12ring0bind ""twist""
gesturecollection.slot12ring1bind """"
gesturecollection.slot1ring0bind ""surrender""
gesturecollection.slot1ring1bind """"
gesturecollection.slot2ring0bind ""hurry""
gesturecollection.slot2ring1bind """"
gesturecollection.slot3ring0bind ""ok""
gesturecollection.slot3ring1bind """"
gesturecollection.slot4ring0bind ""point""
gesturecollection.slot4ring1bind """"
gesturecollection.slot5ring0bind ""shrug""
gesturecollection.slot5ring1bind """"
gesturecollection.slot6ring0bind ""thumbsdown""
gesturecollection.slot6ring1bind """"
gesturecollection.slot7ring0bind ""thumbsup""
gesturecollection.slot7ring1bind """"
gesturecollection.slot8ring0bind ""victory""
gesturecollection.slot8ring1bind """"
gesturecollection.slot9ring0bind ""beatchest""
gesturecollection.slot9ring1bind """"
lookattooltip.crosshairmode ""0""
megaphone.ignorepushtotalk ""True""
metaldetectorsource.draweditorgizmos ""False""
midiconvar.bufferprotection ""False""
midiconvar.debugmode ""False""
midiconvar.enabled ""False""
nametags.enabled ""True""
projectile.preventcameraclip ""True""
recordertool.debugrecording ""False""
rgbeffects.brightness ""0""
rgbeffects.colorcorrection_razer ""(3.00, 3.00, 3.00)""
rgbeffects.colorcorrection_steelseries ""(1.50, 1.50, 1.50)""
rgbeffects.enabled ""False""
keyboardmidi.midikeymap ""qwerty-uk.json""
ui_dropscontroller.show_placeholder_drop_data ""False""
screenshot.hiresscreenshotcustomwidth ""0""
shoutcaststreamer.allowinternetstreams ""False""
shoutcaststreamer.maxaudiostreams ""3""
socket_free_snappable.snappingmode ""2""
strobelight.forceoff ""True""
toolgun.classiceffects ""False""
graphics.shadowlights ""1""
graphicssettings.shadowqualitypreset ""2""
graphicssettings.globaltexturemipmaplimit ""0""
graphics.af ""8""
graphics.lodbias ""1""
graphics.shaderlod ""5""
graphicssettings.anisotropicfiltering ""0""
mesh.quality ""150""
terrain.quality ""100""
graphics.contactshadows ""True""
effects.ao ""True""
tree.meshes ""100""
water.quality ""0""
water.reflections ""2""
grass.displacement ""True""
grass.quality ""100""
graphics.grassshadows ""True""
graphics.volumetric_clouds ""4""
effects.sharpen ""True""
effects.antialiasing ""3""
reflection.planarreflections ""False""");

                SetLaunchOptions("-player.eye_blinking False -player.eye_movement False -client.headlerp 10 -headlerp_inertia 0 -global.enable_marker_teleport True -graphics.shadowmode 1");
                ShowNotify("High settings applied!");
            }
            else
            {
                ShowNotify("Please select a preset!");
                return;
            }
            RestartSteam();
        }
        bool CloseSteamSafe()
        {
            var processes = Process.GetProcessesByName("steam");

            if (processes.Length == 0)
                return true;

            var result = MessageBox.Show(
                "Steam must be closed to apply settings.\nClose Steam now?",
                "Rust Optimizer",
                MessageBoxButton.YesNo);

            if (result != MessageBoxResult.Yes)
                return false;

            foreach (var p in processes)
            {
                try
                {
                    p.Kill(); 
                }
                catch { }
            }

            System.Threading.Thread.Sleep(1500);

            return true;
        }
        void RestartSteam()
        {
            var path = GetSteamPath();
            if (path == null) return;

            var exe = Path.Combine(path, "steam.exe");

            if (File.Exists(exe))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = exe,
                    UseShellExecute = true
                });
            }
        }
        void ShowNotify(string text)
        {
            NotifyText.Text = text;
            NotifyBox.Opacity = 1;

            var transform = NotifyBox.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform(0, 20);
                NotifyBox.RenderTransform = transform;
            }

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            var moveUp = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(200));

            NotifyBox.BeginAnimation(OpacityProperty, fadeIn);
            transform.BeginAnimation(TranslateTransform.YProperty, moveUp);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, e) =>
            {
                timer.Stop();

                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                NotifyBox.BeginAnimation(OpacityProperty, fadeOut);
            };
            timer.Start();
        }
    }
}