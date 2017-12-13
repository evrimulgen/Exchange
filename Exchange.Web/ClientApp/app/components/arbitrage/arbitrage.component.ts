import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

import { ArbitrageResult } from '../../../models/arbitrageresult';

@Component({
    selector: 'arbitrage',
    templateUrl: './arbitrage.component.html'
})
export class ArbitrageComponent {

    public arbitrages: ArbitrageResult[];
    public http: Http;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        http.get(baseUrl + 'api/Arbitrage/Get').subscribe(result => {
            this.arbitrages = result.json() as ArbitrageResult[];
        }, error => console.error(error));
    }

    selectedArbitrage: any;

    outcome: any;

    onSelect(arbitrage: any, http: Http, @Inject('BASE_URL') baseUrl: string): void {
        this.selectedArbitrage = "";
        this.outcome = "white";
        this.http.get('api/Arbitrage/Details?symbol=' + arbitrage.symbol + '&market=' + arbitrage.market + '&exchange1=' + arbitrage.exchange1 + '&exchange2=' + arbitrage.exchange2).subscribe(result => {
            this.selectedArbitrage = result.json();
            this.outcome = (this.selectedArbitrage.low.orders.market.volume > 0 && this.selectedArbitrage.high.orders.market.volume > 0) && 
                (this.selectedArbitrage.low.orders.sell[0].price < this.selectedArbitrage.high.orders.buy[0].price)? "green" : "red";
        }, error => console.error(error));
    }
}