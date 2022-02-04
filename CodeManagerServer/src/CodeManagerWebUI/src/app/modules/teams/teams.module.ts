import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { TeamComponent } from './components/team/team.component';
import { CreateTeamComponent } from './components/create-team/create-team.component';

@NgModule({
  declarations: [TeamComponent, CreateTeamComponent],
  imports: [CommonModule, SharedModule],
  exports: [CreateTeamComponent, TeamComponent],
})
export class TeamsModule {}
