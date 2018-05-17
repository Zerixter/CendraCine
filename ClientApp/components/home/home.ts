import Vue from 'vue';
import { Component } from 'vue-property-decorator';

interface Movie {
    name: string;
    synopsis: string;
    trailer: string;
    cover: string;
    recommendedAge: number;
}

@Component
export default class HomeComponent extends Vue {
    movies: Movie[] = [];

    mounted() {
        fetch('api/movie')
            .then(response => response.json() as Promise<Movie[]>)
            .then(data => {
                this.movies = data;
            });
    }
}
