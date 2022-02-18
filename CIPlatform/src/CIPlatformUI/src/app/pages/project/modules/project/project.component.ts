import {
  Component,
  Input,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewChild,
} from '@angular/core';
import {
  CreateProjectDto,
  ProjectDataDto,
} from '../../../../services/project/project.interface';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../../../../services/project/project.service';
import { MemberPermission } from '../../../../services/team/team.interface';
import { Select } from '@ngxs/store';
import { Observable } from 'rxjs';
import { RunService } from '../../../../services/run/run.service';
import { RunDto, State } from '../../../../services/run/run.interface';
import {
  ResultsChannel,
  SignalRService,
} from '../../../../services/signalr/signalr.service';
import { ProcessedRunResult } from '../../../../services/run/event-result.interface';
import { BaseRunComponent } from '../../../../shared/components/base-run.component';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss'],
})
export class ProjectComponent
  extends BaseRunComponent
  implements OnInit, OnDestroy
{
  currentTemplate?: TemplateRef<any>;

  State = State;

  @ViewChild('editProjectTemplate') editProjectTemplate?: TemplateRef<any>;
  @ViewChild('deleteProjectTemplate') deleteProjectTemplate?: TemplateRef<any>;
  @ViewChild('startRunTemplate') startRunTemplate?: TemplateRef<any>;

  @Input() projectData?: ProjectDataDto;
  showWindow: boolean = false;

  @Select((state: any) => state.app.user.user.isAdmin)
  isAdmin?: Observable<boolean>;

  elapsedExecutionTime: number = 0;
  interval?: number = undefined;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly projectService: ProjectService,
    private readonly signalrService: SignalRService,
    runService: RunService,
  ) {
    super(runService);
  }

  ngOnInit(): void {
    const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
    this.projectService
      .getProject(id)
      .subscribe((projectData: ProjectDataDto) => {
        this.projectData = projectData;
        this.listen();
      });
  }

  ngOnDestroy(): void {
    if (this.projectData) {
      this.signalrService.unSubscribeFromResultsChannel(
        ResultsChannel.PROJECT,
        this.projectData.project!.id,
      );
    }
    clearInterval(this.interval);
  }

  listen(): void {
    this.signalrService.registerMethod('ReceiveRunResultEvent', (event: any) =>
      this.onReceiveRunResultEvent(event),
    );
    this.signalrService.registerMethod('ReceiveRunQueuedEvent', (event: any) =>
      this.onReceiveRunQueuedEvent(event),
    );
    this.signalrService.subscribeToResultsChannel(
      ResultsChannel.PROJECT,
      this.projectData!.project!.id,
    );
  }

  onReceiveRunQueuedEvent(runDto: RunDto): void {
    this.projectData?.runs.push(runDto);
  }

  onReceiveRunResultEvent(runResultEvent: ProcessedRunResult): void {
    const selectedRun = this.projectData?.runs.find(
      (run) => run.id === runResultEvent.runId,
    );

    super.onReceiveResultEvent(selectedRun, runResultEvent);
  }

  openDeleteWindow(): void {
    this.toggleWindow(true, this.deleteProjectTemplate);
  }

  openEditWindow(): void {
    this.toggleWindow(true, this.editProjectTemplate);
  }

  openStartRunWindow(): void {
    this.toggleWindow(true, this.startRunTemplate);
  }

  onEdit(createProjectDto: CreateProjectDto): void {
    this.projectService
      .updateProject(createProjectDto, this.projectData!.project.id)
      .subscribe(() => {
        this.projectData = {
          ...(this.projectData as any),
          project: {
            ...(this.projectData?.project as any),
            isPrivateProject: createProjectDto.isPrivateProject,
            description: createProjectDto.description,
            name: createProjectDto.name,
          },
          repositoryUrl: createProjectDto.repositoryUrl,
        };
        this.toggleWindow(false);
      });
  }

  onDelete(): void {
    this.projectService.deleteProject(this.projectData!.project.id).subscribe({
      next: () => {
        this.router.navigate(['projects']);
      },
    });
  }

  onStart(file: File | null): void {
    if (file) {
      this.runService
        .startRun(this.projectData!.project.id, file)
        .subscribe((runDto: RunDto) => {
          this.projectData?.runs.push(runDto);
          this.toggleWindow(false)
        });
    }
  }

  toggleWindow(show: boolean, template?: TemplateRef<any>): void {
    this.showWindow = show;
    if (this.showWindow) {
      this.currentTemplate = template;
    } else {
      this.currentTemplate = undefined;
    }
  }

  async openRun(id: number): Promise<void> {
    await this.router.navigate([`runs/${id}`]);
  }

  showActions(): Observable<boolean> {
    return new Observable<boolean>((observer) => {
      this.isAdmin?.subscribe({
        next: (value: boolean) => {
          observer.next(
            value ||
              this.projectData?.userPermission === MemberPermission.ADMIN ||
              this.projectData?.userPermission === MemberPermission.READ_WRITE,
          );
        },
      });
    });
  }
}
