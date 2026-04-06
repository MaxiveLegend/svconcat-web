export default class VideoPlayer {
    container;
    playButton = null;
    thumbnail = null;
    videoElement = null;
    playVideoBound = null;
    
    constructor(element) {
        console.log("Initializing video...")
        this.container = element;

        this.playButton = this.container.querySelector<HTMLElement>('.c-video__play-button');
        this.thumbnail = this.container.querySelector<HTMLElement>('.c-video__thumbnail');
        this.videoElement = this.container.querySelector<HTMLElement>('video');

        this.bindEvents();
    }

    bindEvents() {
        if (this.playButton) {
            this.playVideoBound = () => this.playVideo();
            this.playButton.addEventListener("click", this.playVideoBound);
        }
    }

    playVideo() {
        console.log("Playing video...");
        if (this.thumbnail) {
            this.thumbnail.classList.add('.u-visually-hidden');
        }

        if (this.playButton) {
            this.playButton.classList.add('.u-visually-hidden');
        }
        
        this.videoElement.play();
    }
}