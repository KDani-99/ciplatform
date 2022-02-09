import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { TeamDataDto } from '../../../../services/team/team.interface';
import { SignalRService } from '../../../../services/signalr/signalr.service';
import { ActivatedRoute, Route } from '@angular/router';

@Component({
  selector: 'app-job',
  templateUrl: './job.component.html',
  styleUrls: ['./job.component.scss'],
})
export class JobComponent implements OnInit {
  currentTemplate?: TemplateRef<any>;
  showWindow: boolean = false;
  @ViewChild('jobConsole') jobConsole?: TemplateRef<any>;
  constructor(
    private readonly route: ActivatedRoute,
    private readonly signalrService: SignalRService,
  ) {}

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
  }

  openConsoleWindow(): void {
    this.toggleWindow(true, this.jobConsole);
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }
}
