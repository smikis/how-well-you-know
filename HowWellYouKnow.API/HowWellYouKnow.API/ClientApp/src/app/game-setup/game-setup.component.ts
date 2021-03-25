import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CreateQuestionDialogComponent } from '../question-dialog/create-question-dialog.component';
import { UserDto } from '../dtos/user-dto.mode';
import { QuestionDto } from '../dtos/question-dto.model';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-game-setup',
  templateUrl: './game-setup.component.html',
  styleUrls: ['./game-setup.component.css']
})
export class GameSetupComponent implements OnInit {
  private hubConnection: signalR.HubConnection;

  constructor(
    @Inject('BASE_URL') private baseUrl: string,
    private http: HttpClient,
    private _snackBar: MatSnackBar,
    private dialog: MatDialog) {
  }

  ngOnInit(): void {
     this.startQuestionsConnection();
     this.addQuestionsListener();

     this.startUsersConnection();
     this.addUsersListener();
  }

  @Input() gameId: string;
  @Input() gameName: string;
  @Input() joinedUsers: UserDto[];
  @Input() questions: QuestionDto[];

  @Output() questionAdded = new EventEmitter<QuestionDto>();
  @Output() userAdded = new EventEmitter<UserDto>();

  public startQuestionsConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'questions')
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public addQuestionsListener = () => {
    this.hubConnection.on(this.gameId, (data: QuestionDto) => {
      console.log(data);
      this.questionAdded.emit(data);
    });
  }

  public startUsersConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + 'users')
      .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public addUsersListener = () => {
    this.hubConnection.on(this.gameId, (data: UserDto) => {
      console.log(data);
      this.userAdded.emit(data);
    });
  }

  startGame() {
    this.http.post<string>(this.baseUrl + 'api/game/' + this.gameId + '/start', {}).subscribe(result => {
      console.log(result);
    }, error => this._snackBar.open(JSON.stringify(error.message)));
  }

  createNewQuestion() {
    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      gameId: this.gameId
    };

    this.dialog.open(CreateQuestionDialogComponent, dialogConfig);
  }


}
