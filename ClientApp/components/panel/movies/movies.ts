import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component
export default class MoviesComponent extends Vue {
    currentcount: number = 0;

    incrementCounter() {
        this.currentcount++;
    }
}
