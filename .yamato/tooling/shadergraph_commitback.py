import argparse
import os
from os import path
import platform
from git import Repo, exc
from shutil import copyfile
from subprocess import run

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Takes the output from shadergraph's image tests and commits them "
                                                 "back to the source repo")
    parser.add_argument("--root")
    parser.add_argument("--colorspace")
    parser.add_argument("--platform")
    parser.add_argument("--api")
    parser.add_argument("--vr", default="None")

    args = parser.parse_args()

    repo = Repo(".\\")
    branch_name = os.getenv("GIT_BRANCH")
    if branch_name is None:  # Local run, we can use the branch name
        branch_name = repo.active_branch
    repo.git.stash()
    new_branch_name = branch_name.name + "-ref-images"
    repo.create_head(new_branch_name)
    repo.git.checkout(new_branch_name)
    try:
        repo.git.stash("pop")
    except exc.GitCommandError:
        pass

    with open(path.join(args.root, "UpdateTests.txt")) as f:
        while True:
            line = f.readline().strip()
            if line == "":
                break
            test_name, asset_path, should_update_image = line.split(",")

            if should_update_image is "True":
                actual_img_path = path.join(os.getcwd(), args.root, "Assets", "ActualImages",
                                            args.colorspace, args.platform, args.api, args.vr, test_name + ".png")
                reference_img_path = path.join(os.getcwd(), args.root, "Assets", "ReferenceImages",
                                               args.colorspace, args.platform, args.api, args.vr, test_name)
                copyfile(actual_img_path, reference_img_path + ".png")
                # print(reference_img_path)
                run(['git', 'add', reference_img_path + ".png"], shell=True)
                run(['git', 'add', reference_img_path + ".png.meta"], shell=True)
                run(['git', 'add', reference_img_path + ".meta"], shell=True)
                # repo.index.add([reference_img_path])

            # repo.index.add([path.join(args.root, asset_path)])
            full_asset_path = path.join(os.getcwd(), args.root, asset_path)
            # print(full_asset_path)
            run(['git', 'add', full_asset_path], shell=True)
            run(['git', 'add', full_asset_path + ".meta"], shell=True)

    repo.git.commit("-m", "Generated reference images for " + platform.system())
    repo.remote(name="origin").push()
