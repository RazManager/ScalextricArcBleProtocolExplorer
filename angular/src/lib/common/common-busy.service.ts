import { Injectable, Output, EventEmitter } from '@angular/core';


@Injectable()
export class CommonBusyService {
    @Output() public onBusyChanged: EventEmitter<boolean> = new EventEmitter(true);
    private services = new Array<any>();


    public begin(service: any): void {
        if (!this.services.find(x => x === service)) {
            this.services.push(service);
        }
        this.onBusyChanged.emit(true);
    }


    public end(service: any): void {
        const idx = this.services.findIndex(x => x === service)
        if (idx >= 0) {
            this.services.splice(idx, 1);
        }
        if (this.services.length === 0) {
            this.onBusyChanged.emit(false);
        }
    }
}
