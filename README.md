<br/>
<p align="center">
  <a href="https://github.com/LazyDB-community/LazyCSharp">
    <img src="https://i.imgur.com/oTW6Hab.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">LazyCSharp</h3>

  <p align="center">
    Connector for LazyDB in CSharp.
    <br/>
    <br/>
    <a href="https://github.com/LazyDB-community/LazyCSharp"><strong>Explore the docs »</strong></a>
    <br/>
    <br/>
    <a href="https://github.com/LazyDB-community/LazyCSharp">View Demo</a>
    .
    <a href="https://github.com/LazyDB-community/LazyCSharp/issues">Report Bug</a>
    .
    <a href="https://github.com/LazyDB-community/LazyCSharp/issues">Request Feature</a>
  </p>
</p>

![Downloads](https://img.shields.io/github/downloads/LazyDB-community/LazyCSharp/total) ![Contributors](https://img.shields.io/github/contributors/LazyDB-community/LazyCSharp?color=dark-green) ![Forks](https://img.shields.io/github/forks/LazyDB-community/LazyCSharp?style=social) ![Stargazers](https://img.shields.io/github/stars/LazyDB-community/LazyCSharp?style=social) ![Issues](https://img.shields.io/github/issues/LazyDB-community/LazyCSharp) ![License](https://img.shields.io/github/license/LazyDB-community/LazyCSharp) 

## Table Of Contents

* [Requirements](#requirements)
* [License](#license)
* [Authors](#authors)
* [Acknowledgements](#acknowledgements)

## Requirements

* A working LazyDB server, that you can get on https://lazydb.com    
* A C# project compatible with .NET 5.0

## Getting Started

Using LazyDB in C# is really easy, you only need to create a Database object!

```c#
Database db = new Database("youtproject.lazydb.com", 42600, delegate (Object s) {
    Console.WriteLine("Connection to your LazyDB server established!");
}, delegate (Object s) {
    Console.WriteLine("connection to your LazyDB server is no longer established!");
});
```

Once you initialized the database, you can use any command, here is an example of the login command :

```c#
Callback callback = new Callback();
callback.success = delegate (Newtonsoft.Json.Linq.JToken s) {
    Console.WriteLine(s);
};

callback.fail = delegate (Newtonsoft.Json.Linq.JToken s) {
    Console.WriteLine(s);
};

db.connect("email", "password", callback);
```

## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.
* If you have suggestions for adding or removing projects, feel free to [open an issue](https://github.com/LazyDB-community/LazyCSharp/issues/new) to discuss it, or directly create a pull request after you edit the *README.md* file with necessary changes.
* Please make sure you check your spelling and grammar.
* Create individual PR for each suggestion.
* Please also read through the [Code Of Conduct](https://github.com/LazyDB-community/LazyCSharp/blob/main/CODE_OF_CONDUCT.md) before posting your first idea as well.

### Creating A Pull Request

1. Fork the Project
2. Create your Feature Branch
3. Commit your Changes
4. Push to the Branch
5. Open a Pull Request

## License

Distributed under the MIT License. See [LICENSE](https://github.com/LazyDB-community/LazyCSharp/blob/main/LICENSE.md) for more information.

## Authors

* **MoskalykA** - *Developer* - [MoskalykA](https://github.com/MoskalykA/) - *Part of the library*
* **Vic92548** - *CTO* - [Vic92548](https://github.com/Vic92548) - *Part of the library*
