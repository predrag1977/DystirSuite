const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:20711';

const context = [
    "/dystirhub",
    "/api/matches",
    "/api/refresh",
    "/api/standings",
    "/api/statistics",
    "/api/matchdetails",
    "/api/sponsors",
    "/data/request/numberofmatches",
    "/data/request/countmatches",
    "/data/request/datapackage",
    "/data/request/matches",

    //API for DystirManager
    "/api/Login",
    "/api/Matches",
    "/api/MatchDetails",
    "/api/Administrators",
    "/api/EventsOfMatches",
    "/api/PlayersOfMatches",
    "/api/Teams",
    "/api/Categories",
    "/api/MatchTypes",
    "/api/Squads",
    "/api/Statuses",
    "/api/Rounds"
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
