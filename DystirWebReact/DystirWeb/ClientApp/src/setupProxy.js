const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:20711';

const context = [
    "/weatherforecast",
    "/dystirhub",
    "/api/matches",
    "/api/matches/results",
    "/api/refresh",
    "/api/Login",
    "/api/standings",
    "/api/matchDetails",
    "/api/eventsOfMatches",
    "/api/playersOfMatches",
    "/api/Administrators/token",
    "/api/teams",
    "/api/categories",
    "/api/matchTypes",
    "/api/squads",
    "/api/statuses",
    "/api/rounds"
];

const onError = (err, req, resp, target) => {
    console.error(`${err.message}`);
}

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        target: target,
        // Handle errors to prevent the proxy middleware from crashing when
        // the ASP NET Core webserver is unavailable
        onError: onError,
        secure: false,
        // Uncomment this line to add support for proxying websockets
        ws: true,
        //headers: {
        //  Connection: 'Keep-Alive'
        //}
    });

  app.use(appProxy);
};
