# NetCoreServer
create fast and low latency asynchronous socket server<br><br>

# 고성능 네트워크 관련 글
<h3>.NET Core의 비동기 네트워크 라이브러리 성능 개선</h3>
<ul>
  <li>닷넷은 현재 버전 8이 나왔다.(2023.11.15)</li>
    <ul>
      <li>성장한 만큼 많은 개선과 기능 추가가 있었다. 그러나 프로그래머들이 닷넷의 성장을 따라가지 못하는 경우가 많다.</li>
      <li>MMORPG장르의 게임 서버를 .NET Core로 개발한 게임이 점차 늘어나고 있다.</li>
    </ul>
  <li>닷넷으로 고성능 네트워크 프로그래밍을 하기 위해 자료를 찾아본다면 아래 소개한 글을 읽기 바란다.</li>
</ul>

<h3>SocketAsyncEventArgs</h3>
<ul>
  <li>닷넷에서 고성능 네트워크 프로그래밍을 하기 위해서는 필연적으로 3.5에서 추가된 SocketAsyncEventArgs 클래스에 대해서 알아야 한다.</li>
    <ul><li><a href="http://msdn.microsoft.com/ko-kr/library/system.net.sockets.socketasynceventargs.aspx">SocketAsyncEventArgs</a></li></ul>
  <li>부족한 내용은 다음 CodeProject의 'C# SocketAsyncEventArgs 고성능 소켓 코드'강좌에서 확인할 수 있습니다.</li>
    <ul><li><a href="http://www.codeproject.com/Articles/83102/C-SocketAsyncEventArgs-High-Performance-Socket-Cod">C# SocketAsyncEventArgs 고성능 소켓 코드</a></li></ul>
  <li>.NET 3.5 이전의 통신 방식과 비교하여 개선된 점은 아래의 사이트에서 확인할 수 있다.</li>
    <ul><li><a href="https://stackoverflow.com/questions/14856990/await-async-or-socketasynceventargs">비동기 대기와 SocketAsyncEventArgs</a></li></ul>
  <li>사용법은 msdn에서 샘플 예제를 통해 SocketAsyncEventArgsPool 클래스를 볼 수 있다.</li>
    <ul><li><a href="https://learn.microsoft.com/ko-kr/dotnet/api/system.net.sockets.socketasynceventargs.-ctor?view=net-8.0">SocketAsyncEventArgsPool 샘플</a></li></ul>
</ul>
