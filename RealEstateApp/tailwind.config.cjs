/** @type {import('tailwindcss').Config} */
module.exports = {
  corePlugins: {
    preflight: false
  },
  content: [
    "./RealEstateApp.WebApp/Views/**/*.{cshtml,razor}",
    "./RealEstateApp.WebApp/Pages/**/*.{cshtml,razor}",
    "./RealEstateApp.WebApp/wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {}
  },
  plugins: []
};
