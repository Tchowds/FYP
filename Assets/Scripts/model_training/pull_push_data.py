import shutil
import subprocess

# Utility script to copy the data file from the HMD and copy it back to the HMD

pull = False
push = True
# Defines if development is done on Quest Link or over adb
link = False

# Define the location where the data file is stored when using Quest Link
link_location = r"C:\Users\Taha\AppData\LocalLow\DefaultCompany\VR Hand Tracking test\PoseGestureData.json"
# Define the location where the data file is stored when using adb on the device
adb_location = "/sdcard/Android/data/com.DefaultCompany.VRHandTrackingtest/files/PoseGestureData.json"

def sync_data(pull, push, link, link_location=link_location, adb_location=adb_location):
    """
    pull: bool whether to pull the file from the HMD/Quest Link
    push: bool whether to push the file back to the HMD/Quest Link
    link: bool True for Quest Link (PC path), False for adb (device path)
    """
    if link:
        if pull:
            print("Pulling file from HMD via Quest Link")
            shutil.copy(link_location, "PoseGestureData.json")
        if push:
            print("Pushing file to HMD via Quest Link")
            shutil.copy("PoseGestureData.json", link_location)
    else:
        if pull:
            print("Pulling file from HMD via adb")
            subprocess.run(["adb", "pull", adb_location])
        if push:
            print("Pushing file to HMD via adb")
            subprocess.run(["adb", "push", "PoseGestureData.json", adb_location])

# call the function

def main():
    sync_data(pull, push, link, link_location, adb_location)

if __name__ == "__main__":
    main()
