import { Injectable } from '@angular/core';
import { BreakpointObserver, BreakpointState, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';


@Injectable()
export class CommonBreakpointObserver extends BreakpointObserver {
    public get observeNarrow() : Observable<BreakpointState> {
        return this.observe([Breakpoints.XSmall, Breakpoints.Small]);
    }


    public get isNarrow() : boolean {
        return this.isMatched(Breakpoints.XSmall) || this.isMatched(Breakpoints.Small);
    }

        
    public get isLeXSmall() : boolean {
        return this.isMatched(Breakpoints.XSmall);
    }


    public get isLeSmall() : boolean {
        return this.isMatched(Breakpoints.XSmall) || this.isMatched(Breakpoints.Small);
    }


    public get isLeMedium() : boolean {
        return this.isMatched(Breakpoints.XSmall) || this.isMatched(Breakpoints.Small) || this.isMatched(Breakpoints.Medium);
    }

        
    public get isLeLarge() : boolean {
        return this.isMatched(Breakpoints.XSmall) || this.isMatched(Breakpoints.Small) || this.isMatched(Breakpoints.Medium) || this.isMatched(Breakpoints.Large);
    }

        
    public get isPointerDevice(): boolean {
        return window.matchMedia('(pointer: fine)').matches;
    }
}
