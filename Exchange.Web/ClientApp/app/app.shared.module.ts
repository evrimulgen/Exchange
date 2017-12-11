import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { ArbitrageComponent } from './components/arbitrage/arbitrage.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        NavMenuComponent,
        ArbitrageComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'arbitrage', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'arbitrage', component: ArbitrageComponent },
            { path: '**', redirectTo: 'arbitrage' }
        ])
    ]
})
export class AppModuleShared {
}
