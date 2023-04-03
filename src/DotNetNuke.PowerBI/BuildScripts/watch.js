/* eslint-disable no-console */
const pkg = require('../package.json');
const fs = require('fs-extra');
const path = require('path');
const chokidar = require('chokidar');

function copy(srcDir, srcRelativePath, destDir) {
    const fullSrcPath = path.join(srcDir, srcRelativePath);
    const fullDestPath = path.join(destDir, srcRelativePath);

    return fs
        .ensureDir(path.dirname(fullSrcPath))
        .then(() => fs.copy(fullSrcPath, fullDestPath));
}

function copyContainer(srcDir, srcRelativePath, destDir) {
    const fullSrcPath = path.join(srcDir, srcRelativePath.replace('containers', ''));
    const fullDestPath = path.join(destDir, srcRelativePath.replace('containers', ''));

    return fs
        .ensureDir(path.dirname(fullSrcPath))
        .then(() => fs.copy(fullSrcPath, fullDestPath));
}

const relative = srcFullPath => path.relative(srcDir, srcFullPath);

const watchModule = () => {
    const destModuleDir = `${pkg.dnn.dnnRoot}/DesktopModules/${pkg.dnn.folderName}/**`;

    const moduleWatcher = chokidar.watch(srcDir, {
        ignoreInitial: true,
        ignored: ignoredFiles,
    });

    moduleWatcher.on('add', path => {
        copy(srcDir, relative(path), destModuleDir)
            .then(() => console.log({ src: 'Module Watcher', relative: relative(path), type: 'add' }))
            .catch(reason => console.log(reason));
    });

    moduleWatcher.on('change', path => {
        copy(srcDir, relative(path), destModuleDir)
            .then(() => console.log({ src: 'Module Watcher', relative: relative(path), type: 'change' }))
            .catch(reason => console.log(reason));
    });
}

const watchSkin = () => {
    const destSkinDir = `${pkg.dnn.dnnRoot}/portals/_default/skins/${pkg.dnn.folderName}/**`;

    // Watch skin files (ignores containers)
    const skinWatcher = chokidar.watch(srcDir, {
        ignoreInitial: true,
        ignored: ignoredFiles.concat(['**/containers']),
    });

    skinWatcher.on('add', path => {
        // console.log('SKIN_WATCHER', 'ADD', 'destSkinDir', destSkinDir);

        copy(srcDir, relative(path), destSkinDir)
            .then(() => console.log({ src: 'Skin Watcher', relative: relative(path), type: 'add' }))
            .catch(reason => console.log(reason));
    });

    skinWatcher.on('change', path => {
        // console.log('SKIN_WATCHER', 'CHANGE', 'destSkinDir', destSkinDir);

        copy(srcDir, relative(path), destSkinDir)
            .then(() => console.log({ src: 'Skin Watcher', relative: relative(path), type: 'change' }))
            .catch(reason => console.log(reason));
    });
}

const watchContainers = () => {
    const destContainersDir = `${pkg.dnn.dnnRoot}/portals/_default/containers/${pkg.dnn.folderName}/**`;

    // Watch container files
    const containerWatcher = chokidar.watch(srcContainerDir, {
        ignoreInitial: true,
        ignored: ignoredFiles,
    });

    containerWatcher.on('add', path => {
        copyContainer(srcContainerDir, relative(path), destContainersDir)
            .then(() => console.log({ src: 'Containers Watcher', relative: relative(path), type: 'add' }))
            .catch(reason => console.log(reason));
    });

    containerWatcher.on('change', path => {
        copyContainer(srcContainerDir, relative(path), destContainersDir)
            .then(() => console.log({ src: 'Containers Watcher', relative: relative(path), type: 'change' }))
            .catch(reason => console.log(reason));
    });
}

const watchBin = () => {
    const assemblyWatcher = chokidar.watch(`${binDir}/${pkg.dnn.assemblyName}.{dll,pdb}`, {
        ignoreInitial: true,
        ignored: [
            'Dnn*',
            'DotNetNuke*',
            'System*',
            'EntityFramework*',
            'Microsoft*',
            'Newtonsoft*',
            '*.deps.json',
        ],
    });

    assemblyWatcher.on('add', file => {
        copy(binDir, path.relative(binDir, file), destAssemblyDir)
            .then(() => console.log({ src: 'Assembly Watcher', relative: path.relative(binDir, file), type: 'add' }))
            .catch(reason => console.log(reason));
    });

    assemblyWatcher.on('change', file => {
        copy(binDir, path.relative(binDir, file), destAssemblyDir)
            .then(() => console.log({ src: 'Assembly Watcher', relative: path.relative(binDir, file), type: 'change' }))
            .catch(reason => console.log(reason));
    });
}

const srcDir = './**';
const srcContainerDir = './containers/**';
const binDir = './bin';
let destAssemblyDir = `${pkg.dnn.dnnRoot}/bin/`;
let ignoredFiles = ['**/*.{pdb,dll,sql,dnn,csproj,csproj.user,txt,config,json,cs}', 'BuildScripts', 'bin', 'obj', 'node_modules'];
// eslint-disable-next-line prettier/prettier


if (pkg.dnn.mode === 'module') {
    watchModule();
}

if (pkg.dnn.mode === 'skin') {
    watchSkin();
    watchContainers();
}

if (pkg.dnn.copyBin) {
    watchBin();
}
