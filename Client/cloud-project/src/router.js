import Vue from "vue";
import Router from "vue-router";

import HomeRouter from "./pages/home/router";

Vue.use(Router);

var allRoutes = [];

const redirectToHome = [
  {
    path: "/",
    redirect: { name: "home.index" },
  },
];

allRoutes = allRoutes.concat(HomeRouter, redirectToHome);

const routes = allRoutes;

export default new Router({
  mode: "history",
  routes: routes,
});
