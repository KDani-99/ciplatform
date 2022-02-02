import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamsComponent } from './teams.component';
import { SharedModule } from '../../shared/shared.module';
import { TeamComponent } from './components/team/team.component';

@NgModule({
  declarations: [TeamsComponent, TeamComponent],
  imports: [CommonModule, SharedModule],
})
export class TeamsModule {}
