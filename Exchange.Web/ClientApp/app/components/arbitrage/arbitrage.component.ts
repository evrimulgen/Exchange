import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';


@Component({
    selector: 'arbitrage',
    templateUrl: './arbitrage.component.html'
})
export class ArbitrageComponent {
    public arbitrages: ArbitrageResult[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/Arbitrage/Get').subscribe(result => {
            this.arbitrages = result.json() as ArbitrageResult[];
        }, error => console.error(error));
    }
}

interface ArbitrageResult {
    market: string;
    symbol: string;
    exchange1: string;
    exchange1Price: number;
    exchange2: string;
    exchange2Price: number;
    percentage: number;
    exchange1Link: string;
    exchange2Link: string;
}
