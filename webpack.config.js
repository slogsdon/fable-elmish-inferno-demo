const path = require('path');
const webpack = require('webpack');
const fableUtils = require('fable-utils');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');

function resolve(filePath) {
  return path.join(__dirname, filePath)
}

const babelOptions = fableUtils.resolveBabelOptions({
  presets: [['es2015', { 'modules': false }]],
  plugins: ['transform-runtime']
});

const isProduction = process.argv.indexOf('-p') >= 0;
console.log('Bundling for ' + (isProduction ? 'production' : 'development') + '...');

module.exports = {
  devtool: 'source-map',
  entry: {
    bundle: resolve('./src/Site.Client/Site.Client.fsproj'),
  },
  output: {
    filename: '[name].[hash].js',
    path: resolve('./public'),
    publicPath: '/',
  },
  resolve: {
    modules: [resolve('./node_modules/')]
  },
  devServer: {
    contentBase: resolve('./public'),
    port: 8080
  },
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: 'fable-loader',
          options: {
            babel: babelOptions,
            define: isProduction ? [] : ['DEBUG']
          }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: babelOptions
        },
      }
    ]
  },
  plugins: [
    new CopyWebpackPlugin([
      { from: './src/Site.Client/static' }
    ]),
    new HtmlWebpackPlugin({
      inject: true,
      template: './src/Site.Client/static/index.html',
    }),
  ]
};
