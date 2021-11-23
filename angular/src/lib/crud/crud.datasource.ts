import { DataSource } from '@angular/cdk/collections';
import { Observable,  BehaviorSubject } from 'rxjs';


export class CrudBehaviorSubjectDataSource<TDto> extends DataSource<TDto> {
    private subjectEntities: BehaviorSubject<TDto[]>;


    constructor(private readonly entities: TDto[]) {
        super();
        this.subjectEntities = new BehaviorSubject(entities);
    }


    public update(entities: TDto[]) {
        this.subjectEntities.next(entities);
    }


    public connect(): Observable<TDto[]> {
        return this.subjectEntities.asObservable();
    }


    public disconnect() {}
}
