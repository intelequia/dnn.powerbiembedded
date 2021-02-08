const webpack = require("webpack");
const webpackExternals = require("@dnnsoftware/dnn-react-common/WebpackExternals");
const path = require("path");
const packageJson = require("./package.json");
const isProduction = process.env.NODE_ENV === "production";
const languages = {
    "en": null
    // TODO: create locallizaton files per language 
    // "de": require("./localizations/de.json"),
    // "es": require("./localizations/es.json"),
    // "fr": require("./localizations/fr.json"),
    // "it": require("./localizations/it.json"),
    // "nl": require("./localizations/nl.json")
};

module.exports = {
    entry: "./src/main.jsx",
    optimization: {
        minimize: isProduction
    },    
    output: {
        path: path.resolve(__dirname, "../admin/personaBar/scripts/bundles/"),
        filename: "bundle-en.js",
        publicPath: isProduction ? "" : "http://localhost:8080/dist/scripts/bundles"
    },

    resolve: {
        extensions: [".js", ".json", ".jsx"],
        modules: [
            path.resolve('./src'),           // Look in src first
            path.resolve('./node_modules')  // Try local node_modules
        ]
    },    
    module: {
        rules: [
            { 
                test: /\.(js|jsx)$/, 
                exclude: /node_modules/, 
                enforce: "pre",
                loader: 'eslint-loader',
                options: { fix: true }
            },
            { test: /\.(js|jsx)$/, exclude: /node_modules/, loader: "babel-loader" },            
            { 
                test: /\.(less|css)$/,
                use: [
                    { loader: "style-loader" },
                    { loader: "css-loader", options: { modules: "global" } },
                    { loader: "less-loader" }
                  ]
            },
            { test: /\.(ttf|woff)$/, loader: "url-loader?limit=8192" },
            { test: /\.(gif|png)$/, loader: "url-loader?mimetype=image/png" }
        ]
    }, 
    externals: webpackExternals,
    plugins: isProduction ? [
        new webpack.DefinePlugin({
            VERSION: JSON.stringify(packageJson.version),
            "process.env": {
                "NODE_ENV": JSON.stringify("production")
            }
        })
    ] : [
            new webpack.DefinePlugin({
                VERSION: JSON.stringify(packageJson.version),
                "process.env": {
                    "NODE_ENV": JSON.stringify("development")
                }                
            })
        ],
    devtool: 'source-map'
};