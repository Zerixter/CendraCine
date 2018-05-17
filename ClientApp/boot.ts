import 'bootstrap/dist/css/bootstrap.min.css';
import 'font-awesome/css/font-awesome.css';
import './css/site.css';
import 'jquery';
import 'bootstrap/dist/js/bootstrap.min.js';
import Vue from 'vue';
import VueRouter from 'vue-router';
Vue.use(VueRouter);

const routes = [
    { path: '/', component: require('./components/home/home.vue.html') },
    { path: '/counter', component: require('./components/counter/counter.vue.html') },
    { path: '/fetchdata', component: require('./components/fetchdata/fetchdata.vue.html') },
    { path: '/panel', component: require('./components/panel/panel.vue.html') },
    { path: '/panel/pelicules', component: require('./components/panel/movies/movies.vue.html') }
];

new Vue({
    el: '#app-root',
    router: new VueRouter({ mode: 'history', routes: routes }),
    render: h => h(require('./components/app/app.vue.html'))
});
