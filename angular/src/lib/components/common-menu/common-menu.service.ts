import { Injectable, Output, EventEmitter } from '@angular/core';


@Injectable()
export class CommonMenuService {
    @Output() public onMenuToggle: EventEmitter<void> = new EventEmitter();

    public menuToogle(): void {
        this.onMenuToggle.emit();
    }
}
